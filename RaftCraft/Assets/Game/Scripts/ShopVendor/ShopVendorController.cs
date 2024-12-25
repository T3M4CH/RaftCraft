using System;
using Game.Scripts.Days;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.InteractiveObjects.Interfaces;
using Game.Scripts.InteractiveObjects.UI;
using Game.Scripts.NPC.Fish;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using GTap.Analytics;
using GTapSoundManager.SoundManager;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ShopVendor
{
    public class ShopVendorController : MonoBehaviour, IInteraction
    {
        public event Action OnBuyEvent = () => { };

        [SerializeField] private string vendorName;

        public enum ShopState
        {
            Lock,
            UnLock
        }

        [System.Serializable]
        public class SaveData
        {
            public int count;
            public ShopState state;
        }

        [SerializeField] private SoundAsset buySound;
        [field: SerializeField, FoldoutGroup("View Word")] public Transform PointAim { get; private set; }
        [SerializeField, FoldoutGroup("Events")] private UnityEvent OnLock;
        [SerializeField, FoldoutGroup("Events")] private UnityEvent OnUnLock;
        [SerializeField, FoldoutGroup("Events")] public UnityEvent OnBuy;

        [SerializeField, ReadOnly] private string _saveKey;

        [SerializeField, FoldoutGroup("Settings")] private DropItems _cost;
        [SerializeField, FoldoutGroup("View")] public BuildCostView _viewCost;
        private ShopState _state;

        public ShopState State
        {
            get => _state;
            set
            {
                _state = value;
                switch (_state)
                {
                    case ShopState.Lock:
                        OnLock?.Invoke();
                        _viewCost.gameObject.SetActive(true);
                        break;
                    case ShopState.UnLock:
                        OnUnLock?.Invoke();
                        _viewCost.gameObject.SetActive(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private int _countResource;

        public int CountResource
        {
            get => _countResource;
            private set
            {
                if (State == ShopState.UnLock)
                {
                    return;
                }

                _countResource = Mathf.Clamp(value, 0, int.MaxValue);
                _viewCost.SetCount(_countResource);
                _viewCost.SetProgress(1f - ((float)_countResource / _cost.Count));
                Save();
                if (_countResource == 0 && State == ShopState.Lock)
                {
                    BuyVendor();
                }
            }
        }

        public bool IsAbleEverywhere => false;

        public bool Interaction
        {
            get { return State == ShopState.Lock; }
        }

        public InteractionType CurrentTypeInteraction => InteractionType.Build;

        private const float DefaultDelay = 0.1f;
        private const float DelayMultiplier = 0.95f;
        private float _currentDelay;
        public float DelayAction => _currentDelay;

        private IResourceService _resourceService;
        private GameSave _gameSave;
        private IDayService _dayService;

        [Inject]
        private void Construct(IResourceService resourceService, GameSave gameSave, IDayService dayService)
        {
            _dayService = dayService;
            _resourceService = resourceService;
            _gameSave = gameSave;
        }

        private void Start()
        {
            var iconConfig = Resources.Load<IconConfig>("IconConfig");
            _viewCost.SetResourceSprite(iconConfig.GetIconItem(_cost.ResourceType));
            _currentDelay = DefaultDelay;
            Load();
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_saveKey))
            {
                _saveKey = Guid.NewGuid().ToString();
            }
        }

        private void Load()
        {
            var data = _gameSave.GetData($"Vendor_{_saveKey}", new SaveData()
            {
                count = _cost.Count,
                state = ShopState.Lock
            });
            State = data.state;
            CountResource = data.count;
        }

        private void OnDisable()
        {
            Save();
        }

        private void Save()
        {
            _gameSave.SetData($"Vendor_{_saveKey}", new SaveData()
            {
                count = CountResource,
                state = State
            });
        }

        private void BuyVendor()
        {
            if (buySound)
            {
                buySound.Play();
            }

            GtapAnalytics.BuyNpc(vendorName);
            OnBuyEvent.Invoke();
            OnBuy?.Invoke();
            State = ShopState.UnLock;
        }

        public void Action()
        {
            if (CountResource > 0)
            {
                if (_resourceService.TryRemove(_cost.ResourceType, 1))
                {
                    _currentDelay *= DelayMultiplier;
                    CountResource--;
                    _viewCost.PlayEffect();
                }
            }
        }

        public void EnterInteraction()
        {
            if (!IsAbleEverywhere && _resourceService.HaveCount(_cost.ResourceType, 1) == false)
            {
                _viewCost.PlayEffectNotItems();
            }
        }

        public void ExitInteraction()
        {
            _currentDelay = DefaultDelay;
        }
    }
}
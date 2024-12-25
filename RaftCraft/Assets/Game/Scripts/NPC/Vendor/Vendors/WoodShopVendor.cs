using System;
using Cysharp.Threading.Tasks;
using Game.Prefabs.NPC.Vendors.Log;
using Game.Scripts.Core.Interface;
using Game.Scripts.ResourceController;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Prefabs.NPC.Vendors
{
    public class WoodShopVendor : MonoBehaviour, IGameObserver<ResourceItem>
    {
        [SerializeField] private float delay;
        [SerializeField] private Image fillImage;
        [SerializeField] private GameObject indicator;
        [SerializeField] private MonoLogInteractor logInteractor;
        [SerializeField] private MonoMoneyShelf shelf;

        private bool _isConverting;
        private float _currentTime;
        private MonoLog _log;
        private IGameObservable<ResourceItem> _resourceObservable;

        [Inject]
        private void Construct(IGameObservable<ResourceItem> resObservable)
        {
            _resourceObservable = resObservable;
        }

        public void PerformNotify(ResourceItem data)
        {
            if (!_isConverting)
            {
                _log = logInteractor.TakeWood();

                if (_log)
                {
                    _isConverting = true;
                    indicator.SetActive(true);
                }
                else
                {
                    indicator.SetActive(false);
                }
            }
        }

        private void PerformConvert()
        {
            _currentTime = 0;
            _isConverting = false;
            fillImage.fillAmount = 0;
            
            shelf.AddMoney(1);
            PerformNotify(null);
        }

        private void FixedUpdate()
        {
            if (_isConverting)
            {
                _currentTime += Time.deltaTime;

                fillImage.fillAmount = _currentTime / delay;

                if (_currentTime > delay)
                {
                    PerformConvert();
                }
            }
        }

        private async void Start()
        {
            _resourceObservable.AddObserver(this);

            await UniTask.Delay(TimeSpan.FromSeconds(1));

            PerformNotify(null);
        }

        private void OnDestroy()
        {
            _resourceObservable.RemoveObserver(this);
        }
    }
}
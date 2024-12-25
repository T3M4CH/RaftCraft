using System;
using System.Collections.Generic;
using Game.Scripts.Core.Interface;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.ShopController
{
    public class ShopSystem : MonoBehaviour
    {
        [SerializeField] private ShopData _shopData;
        [SerializeField] private ShopCell _prefabCell;
        [SerializeField] private RectTransform _rectContent;

        [SerializeField, FoldoutGroup("UI")] private Button _buttonClaim;
        [SerializeField, FoldoutGroup("UI")] private Image _imageIconButton;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCountClaim;
        [SerializeField, FoldoutGroup("UI")] private RectTransform _rectPrice;

        [SerializeField, FoldoutGroup("Settings")] private Sprite _spriteUnLockButton;
        [SerializeField, FoldoutGroup("Settings")] private Sprite _spriteLockButton;
        [SerializeField, FoldoutGroup("Settings")] private GameObject _buttonClaimWord;

        private IResourceService _resource;
        private GameSave _gameSave;
        private IGameObservable<ResourceItem> _observable;
        private List<ShopCell> _poolCell = new List<ShopCell>();
        private Dictionary<string, PriceItem> _poolPrice;
        private List<ShopLoot> _loots = new List<ShopLoot>();

        [Inject]
        private void Construct(IResourceService resourceService, GameSave gameSave, IGameObservable<ResourceItem> observable)
        {
            _resource = resourceService;
            _gameSave = gameSave;
            _observable = observable;
            _poolPrice = new Dictionary<string, PriceItem>();
            foreach (var price in _shopData.Price)
            {
                _poolPrice.TryAdd(price.Id, price);
            }
        }

        private void Start()
        {
            Load();
        }
        

        [Button]
        private void Test()
        {
            _loots[0].Add();
        }

        private void Update()
        {
            foreach (var loot in _loots)
            {
                loot.Update();
            }
        }

        private void Load()
        {
            foreach (var item in _shopData.Price)
            {
                var loot = new ShopLoot(item, CreateCell(item), _resource, _gameSave);
                loot.OnChange += LootOnOnChange;
                loot.Load();
                _loots.Add(loot);
            }
        }

        private int CountOutput()
        {
            var result = 0;
            foreach (var loot in _loots)
            {
                result += loot.CountOutput;
            }
            return result;
        }

        private void LootOnOnChange()
        {
            var count = CountOutput();
            if (_buttonClaimWord != null)
            {
                _buttonClaimWord.SetActive(count > 0);
            }
            _buttonClaim.interactable = count > 0;
            _imageIconButton.sprite = count > 0 ? _spriteUnLockButton : _spriteLockButton;
            _textCountClaim.text = $"{count}";
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectPrice);
            Canvas.ForceUpdateCanvases();
        }

        private ShopCell CreateCell(PriceItem item)
        {
            var cell = Instantiate(_prefabCell, _rectContent);
            cell.Init(item);
            _poolCell.Add(cell);
            _observable.AddObserver(cell);
            return cell;
        }

        public void ClaimAll()
        {
            foreach (var loot in _loots)
            {
                loot.Claim();
            }
        }
        private void OnDestroy()
        {
            foreach (var loot in _loots)
            {
                loot.OnChange -= LootOnOnChange;
                loot.Save();
            }
            
            foreach (var cell in _poolCell)
            {
                _observable.RemoveObserver(cell);
            }
        }
    }
}

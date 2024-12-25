using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.NPC.Fish;
using Game.Scripts.NPC.Spawners;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using Game.Scripts.ShopVendor;
using Game.Scripts.Tutorial.Old;
using Game.Scripts.UI.WindowManager;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Tutorial
{
    public class TutorialFishing : MonoTutorialBase
    {
        [SerializeField, FoldoutGroup("Effects")] private LineRendererMove _prefabEffect;
        [SerializeField, FoldoutGroup("Offset")] private Vector3 _offsetPoint;
        [SerializeField, FoldoutGroup("Offset")] private Vector3 _offsetPlayer;
        [SerializeField, FoldoutGroup("Offset")] private Vector3 _offsetMoney;
        [SerializeField, FoldoutGroup("Settings")] private int _fishCount;
        
        private Dictionary<BaseFish, LineRendererMove> _particlePool;
        private int _currentCountFish;
        private FishSpawner _fishSpawner;
        private IPlayerService _playerService;
        private MonoNetInteractor _netInteractor;
        private MonoMoneyShelf _moneyShelf;
        
        private EntityPlayer _player;

        private int _faze;

        private int Faze
        {
            get => _faze;
            set
            {
                switch (_faze)
                {
                    case 1:
                        _fishSpawner.OnDeadFish -= OnFishGive;
                        Clear();
                        break;
                    case 2:
                        _netInteractor.OnRemovedResource -= OnRemoveFish;
                        Destroy(_lineDropFish.gameObject);
                        break;
                    case 3:
                        _resourceService.OnEventAddResource -= ResourceServiceOnOnEventAddResource;
                        _moneyShelf.OnTakeMoney -= MoneyShelfOnOnTakeMoney;
                        Destroy(_lineGiveMoney.gameObject);
                        break;
                }
                _faze = Mathf.Clamp(value, 0, int.MaxValue);
                _gameSave.SetData(SaveConstants.TutorialFishing, _faze);
                switch (_faze)
                {
                    case 0:
                        break;
                    case 1:
                        _currentCountFish = 0;
                        ShowBattleButton(false);
                        _windowManager.SetNameTutorial("Catch fish");
                        _fishSpawner.OnDeadFish += OnFishGive;
                        var list = _fishSpawner.GetFishOnRadius(_player.transform.position);
                        if (list.Count >= _fishCount)
                        {
                            for (var i = 0; i < _fishCount; i++)
                            {
                                if (_particlePool.ContainsKey(list[i]))
                                {
                                    continue;
                                }
                                SetParticle(list[i]);
                            }
                        }
                        break;
                    case 2:
                        _windowManager.SetNameTutorial("Sell fish");
                        _countDropResource = _fishCount;
                        _netInteractor.OnRemovedResource += OnRemoveFish;
                        _lineDropFish = Instantiate(_prefabEffect);
                        _lineDropFish.CountPoint = 2;
                        _lineDropFish.transform.position = Vector3.zero;
                        _lineDropFish.SetStateLine(true);
                        break;
                    case 3:
                        _countGiveMoney = 0;
                        _windowManager.SetNameTutorial("Take Money");
                        _resourceService.OnEventAddResource += ResourceServiceOnOnEventAddResource;
                        _moneyShelf.OnTakeMoney += MoneyShelfOnOnTakeMoney;
                        _lineGiveMoney = Instantiate(_prefabEffect);
                        _lineGiveMoney.CountPoint = 2;
                        _lineGiveMoney.transform.position = Vector3.zero;
                        _lineGiveMoney.SetStateLine(true);
                        break;
                    case 4:
                        _windowManager.SetNameTutorial("");
                        Complete();
                        Destroy(gameObject);
                        break;
                }
            }
        }

        private void ResourceServiceOnOnEventAddResource(EResourceType type, int count, TypeAddResource space)
        {
            if (type != EResourceType.CoinBlue)
            {
                return;
            }
            _countGiveMoney++;
            if (_countGiveMoney >= 3)
            {
                Faze = 4;
            }
        }

        private LineRendererMove _lineDropFish;
        private LineRendererMove _lineGiveMoney;
        private GameSave _gameSave;
        private WindowManager _windowManager;
        
        private int _countDropResource;
        private int _countGiveMoney;

        private ShopVendorController _shop;
        private IResourceService _resourceService;
        
        public override void Initialize(GameSave gameSave, IPlayerService playerSpawner, WindowManager windowManager,
            object extraParams = null)
        {
            _windowManager = windowManager;
            _gameSave = gameSave;
            _playerService = playerSpawner;
            _playerService.AddListener(OnChangePlayer);
            _resourceService = (IResourceService)extraParams;
        }

        private void Start()
        {
            _shop = FindObjectsOfType<ShopVendorController>().Where(tile => tile.name == "ShopFishing").OrderBy(x => x.gameObject.activeSelf).First();
            if (_shop.State == ShopVendorController.ShopState.UnLock)
            {
                Faze = _gameSave.GetData(SaveConstants.TutorialFishing, 1);
            }
            else
            {
                _shop.OnBuyEvent += ShopOnOnBuyEvent;
            }
        }

        private void ShopOnOnBuyEvent()
        {
            _shop.OnBuyEvent -= ShopOnOnBuyEvent;
            Faze = _gameSave.GetData(SaveConstants.TutorialFishing, 1);
        }


        private void OnChangePlayer(EPlayerStates state, EntityPlayer player)
        {
            if (state == EPlayerStates.SpawnPlayer)
            {
                _player = player;
            }
        }

        private void Awake()
        {
            _fishSpawner = FindObjectOfType<FishSpawner>(true);
            _netInteractor = FindObjectOfType<MonoNetInteractor>(true);
            var money = FindObjectsOfType<MonoMoneyShelf>(true);
            foreach (var t in money)
            {
                if (t.ResourceType == EResourceType.CoinBlue)
                {
                    _moneyShelf = t;
                    break;
                }
            }
            _particlePool = new Dictionary<BaseFish, LineRendererMove>();
        }

        [Button]
        private void Test()
        {
            Faze = 1;
        }
        

        [Button]
        private void Clear()
        {
            foreach (var particle in _particlePool)
            {
                Destroy(particle.Value.gameObject);
            }
            
            _particlePool.Clear();
        }

        private void OnFishGive(BaseFish fish)
        {
            if (_particlePool.ContainsKey(fish))
            {
                Destroy(_particlePool[fish].gameObject);
                _particlePool.Remove(fish);
            }

            _currentCountFish++;
            if (_currentCountFish >= _fishCount)
            {
                Faze = 2;
            }
        }

        private void Update()
        {
            if (_player == null)
            {
                return;
            }



            switch (Faze)
            {
                case 1:
                    if (_particlePool.Count == 0)
                    {
                        return;
                    }
                    foreach (var particle in _particlePool)
                    {
                        particle.Value.SetStateLine(true);
                        particle.Value.SetPosition(0, _player.Hips.position + (_offsetPlayer + _offsetPoint));
                        particle.Value.SetPosition(1, particle.Key.transform.position + _offsetPoint);
                        break;
                    }
                    break;
                case 2:
                    _lineDropFish.SetPosition(0, _player.Hips.position + (_offsetPlayer + _offsetPoint));
                    _lineDropFish.SetPosition(1, _netInteractor.transform.position + _offsetPoint);
                    break;
                case 3:
                    _lineGiveMoney.SetPosition(0, _player.Hips.position + (_offsetPlayer + _offsetPoint));
                    _lineGiveMoney.SetPosition(1, _moneyShelf.transform.position + _offsetMoney);
                    break;
            }
        }

        private void OnRemoveFish(bool state)
        {
            if (state == false)
            {
                return;
            }
            _countDropResource--;
            if (_countDropResource == 0)
            {
                Faze = 3;
            }
        }
        
        private void MoneyShelfOnOnTakeMoney(bool state)
        {
            if (state == false)
            {
                return;
            }

            _countGiveMoney++;
            if (_countGiveMoney >= 3)
            {
                Faze = 4;
            }
        }


        private void SetParticle(BaseFish fish)
        {
            var particle = Instantiate(_prefabEffect, fish.transform);
            particle.CountPoint = 2;
            particle.transform.localPosition = Vector3.zero;
            particle.SetStateLine(false);
            _particlePool.TryAdd(fish, particle);
        }

        private void OnDestroy()
        {
            _resourceService.OnEventAddResource -= ResourceServiceOnOnEventAddResource;

            _fishSpawner.OnDeadFish -= OnFishGive;
            _shop.OnBuyEvent -= ShopOnOnBuyEvent;

            if (_playerService != null)
            {
                _playerService.RemoveListener(OnChangePlayer);
            }

            if (_netInteractor != null)
            {
                _netInteractor.OnRemovedResource -= OnRemoveFish;
            }
        }
    }
}

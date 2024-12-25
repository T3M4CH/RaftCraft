using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Player;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;

namespace Game.Scripts.ResourceController.LocalPlayerResources
{
    public class PlayerResources : IDisposable
    {
        private IPlayerService _playerSpawner;
        private EntityPlayer _player;
        private PlayerResourceView _view;
        private IResourceService _resourceService;

        private Dictionary<EResourceType, int> _resources = new Dictionary<EResourceType, int>();

        public Dictionary<EResourceType, int> Resources => _resources;

        private int _maxCountResource;

        private readonly IPlayerUpgradeSettings _upgradeController;

        public PlayerResources(IPlayerService playerSpawner, IPlayerUpgradeSettings upgradeController)
        {
            _playerSpawner = playerSpawner;
            _upgradeController = upgradeController;
            _playerSpawner.AddListener(PlayerSpawnerOnOnSpawnPlayer);
            _upgradeController.OnUpgrade += ValidateCapacity;
        }

        private void PlayerSpawnerOnOnSpawnPlayer(EPlayerStates state, EntityPlayer player)
        {
            if (_player != null)
            {
                _player.StateMachine.OnEnterState -= StateMachineOnOnEnterState;
            }

            _player = player;
            _view = _player.PlayerResources;
            _player.StateMachine.OnEnterState += StateMachineOnOnEnterState;
            Initialize(_upgradeController.GetValue<int>(EPlayerUpgradeType.Capacity));
        }

        private void StateMachineOnOnEnterState(EntityState state, Entity entity)
        {
      
        }

        public void Initialize(int maxCountResources)
        {
            _maxCountResource = maxCountResources;
            UpdateView();
        }

        private void ValidateCapacity(EPlayerUpgradeType type)
        {
            if (type == EPlayerUpgradeType.Capacity)
            {
                _maxCountResource = _upgradeController.GetValue<int>(EPlayerUpgradeType.Capacity);
                UpdateView();
            }
        }

        private void UpdateView()
        {
            _view.SetCount(CountResources(), _maxCountResource);
        }

        private int CountResources()
        {
            return _resources.Values.Sum();
        }

        public bool TryAddResource(EResourceType type, int count)
        {
            if (CountResources() + count > _maxCountResource)
            {
                _view.PlayFail();
                return false;
            }

            _resources.TryAdd(type, 0);

            _resources[type] += count;
            UpdateView();
            return true;
        }

        public int GetSizeInventory()
        {
            return _maxCountResource - CountResources();
        }

        public void RemoveResource(EResourceType type, int count)
        {
            if (_resources.ContainsKey(type) == false)
            {
                return;
            }

            if (HaveResource(type, count))
            {
                _resources[type] -= count;
                UpdateView();
            }
        }

        public bool TryGetResources(EResourceType type, out int count)
        {
            if (_resources.TryGetValue(type, out var resource))
            {
                count = resource;
                return true;
            }

            count = 0;
            return false;
        }

        public IEnumerable<(EResourceType, int)> GetAllResources()
        {
            return _resources.Where(keyPair => keyPair.Value > 0).Select(keyPair => (keyPair.Key, keyPair.Value));
        }

        public void Clear()
        {
            _resources.Clear();
        }
        
        private bool HaveResource(EResourceType type, int count)
        {
            if (_resources.ContainsKey(type) == false)
            {
                return false;
            }

            return _resources[type] >= count;
        }

        public void Dispose()
        {
            _playerSpawner.RemoveListener(PlayerSpawnerOnOnSpawnPlayer);
            _upgradeController.OnUpgrade -= ValidateCapacity;
            if (_player != null)
            {
                _player.StateMachine.OnEnterState -= StateMachineOnOnEnterState;
            }
        }
    }
}
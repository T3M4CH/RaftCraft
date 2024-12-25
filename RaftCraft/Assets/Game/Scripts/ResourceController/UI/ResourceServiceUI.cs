using System;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Pool;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowManager.Windows;
using Reflex.Attributes;
using Reflex.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.ResourceController.UI
{
    public class ResourceServiceUI : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Data")] private ResourceDataUI _data;
        private IResourceService _resourceService;
        private Camera _main;
        private PoolObjects<ResourceObjectUI> _poolCell;
        private PoolObjects<ResourceObjectUI> _fishPoolCell;
        private EntityPlayer _player;
        private IPlayerService _spawner;
        private WindowManager _windowManager;

        [Inject]
        private void Construct(IResourceService resourceService, IPlayerService playerSpawner, WindowManager windowManager)
        {
            _windowManager = windowManager;
            _resourceService = resourceService;
            _poolCell = new PoolObjects<ResourceObjectUI>(_data.PrefabUI, _data.ParentWindow, 10);
            _fishPoolCell = new PoolObjects<ResourceObjectUI>(_data.PrefabFishUI, _data.ParentWindow, 10);
            _main = Camera.main;
            _spawner = playerSpawner;
            _resourceService.OnEventAddResource += PerformAddResource;
            _spawner.AddListener(PlayerSpawnerOnOnSpawnPlayer);
        }

        private void PlayerSpawnerOnOnSpawnPlayer(EPlayerStates state, EntityPlayer player)
        {
            //_spawner.OnSpawnPlayer -= PlayerSpawnerOnOnSpawnPlayer;
            if (state == EPlayerStates.SpawnPlayer)
            {
                _player = player;
            }
        }

        private void PerformAddResource(EResourceType type, int value, TypeAddResource typeLocation)
        {
            if (_player == null || value < 1 || IsUsualFish(type)) return;
            switch (typeLocation)
            {
                case TypeAddResource.Local:
                    CreateCell();
                    break;
                case TypeAddResource.Global:
                    CreateCell();

                    break;
            }

            void CreateCell()
            {
                ResourceObjectUI cell;
                
                if (type == EResourceType.UniversalFish)
                {
                    var resourceWindow = _windowManager.GetWindow<WindowResources>();
                    for (var i = 0; i < value; i++)
                    {
                        cell = _fishPoolCell.GetFree();
                        cell.gameObject.SetActive(true);
                        cell.ShowToUi(type, 1, _main.WorldToScreenPoint(_player.transform.position) + _data.OffsetTargetPosition, resourceWindow.FishCell);
                    }

                    return;
                }

                cell = _poolCell.GetFree();
                cell.gameObject.SetActive(true);
                cell.Show(type, value, _main.WorldToScreenPoint(_player.transform.position) + _data.OffsetTargetPosition);
            }
        }

        private bool IsUsualFish(EResourceType type)
        {
            var value = (int)type;

            return value is >= 2 and <= 12;
        }

        private bool IsResourceWithIcon(EResourceType resourceType) => (int)resourceType <= 12;

        private RectTransform GetTarget(EResourceType resourceType)
        {
            return resourceType switch
            {
                EResourceType.CoinBlue => _data.TargetMoneyPanel,
                EResourceType.Wood => _data.TargetResourcePanel,
                EResourceType.Crystals => _data.TargetDiamondPane,
                _ => null
            };
        }

        private void OnDestroy()
        {
            if (_resourceService != null) _resourceService.OnEventAddResource -= PerformAddResource;
            if (_spawner != null) _spawner.RemoveListener(PlayerSpawnerOnOnSpawnPlayer);
        }
    }
}
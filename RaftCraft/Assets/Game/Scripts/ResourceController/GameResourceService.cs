using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using UnityEngine;

namespace Game.Scripts.ResourceController
{
    public class GameResourceService : IDisposable, IGameResourceService
    {
        private readonly IResourceService _resourceService;
        private readonly IPlayerService _playerService;
        private EPlayerStates _currentPlayerState;


        public GameResourceService(IResourceService resourceService, IPlayerService playerService)
        {
            _resourceService = resourceService;
            _playerService = playerService;
            _playerService.AddListener(ChangePlayerState);
        }



        private void ChangePlayerState(EPlayerStates states, EntityPlayer player)
        {
            if (_currentPlayerState == states)
            {
                return;
            }

            _currentPlayerState = states;

            if (_currentPlayerState == EPlayerStates.PlayerInRaft)
            {
                StartMoveResource();
            }
        }

        private async void StartMoveResource()
        {
            await WaitMoveResource();
        }

        private async UniTask WaitMoveResource()
        {
            var resource = LocalItems;
            var delay = 100;
            foreach (var itemBackPack in resource)
            {
                for (var i = 0; i < itemBackPack.Item2; i++)
                {
                    if (_resourceService.TryRemoveLocal(itemBackPack.Item1, 1))
                    {
                        _resourceService.Add(itemBackPack.Item1, 1);
                        await UniTask.Delay(delay);
                        delay = Mathf.Clamp(delay - 5, 10, 100);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_playerService != null)
            {
                _playerService.RemoveListener(ChangePlayerState);
            }
        }

        public void Add(EResourceType type, int count)
        {
            _resourceService.Add(type, count);
        }

        public bool TryRemove(EResourceType type, int count)
        {
            return _resourceService.TryRemove(type, count);
        }

        public bool HaveCount(EResourceType type, int count)
        {
            return _resourceService.HaveCount(type, count);
        }

        public IEnumerable<(EResourceType, int)> LocalItems
        {
            get
            {
                var result = new List<(EResourceType, int)>();
                foreach (var type in (EResourceType[]) Enum.GetValues(typeof(EResourceType)))
                {
                    if (_resourceService.GetValueLocal(type) > 0)
                    {
                        result.Add((type, _resourceService.GetValueLocal(type)));
                    }
                }
                return result;
            }
        }
    }
}

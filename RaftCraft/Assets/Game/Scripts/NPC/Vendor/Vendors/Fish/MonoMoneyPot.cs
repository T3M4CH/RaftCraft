using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.CollectingResources;
using Game.Scripts.Player.Spawners;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Prefabs.NPC.Vendors
{
    public class MonoMoneyPot : MonoBehaviour
    {
        [SerializeField] private Transform initSpawnPoint;
        [SerializeField] private CollectingGameResource resourceItem;

        private int _currentCount;
        private GameSave _gameSave;
        private IPlayerService _playerService;
        private List<CollectingResourceObject> _moneyPrefabs = new();
        private IGameResourceService _gameResourceService;

        [Inject]
        private void Construct(GameSave gameSave, IGameResourceService gameResourceService, IPlayerService playerService)
        {
            _gameSave = gameSave;
            _gameResourceService = gameResourceService;
        }

        [Button]
        public void AddMoney(int count, bool save = true)
        {
            for (var i = 0; i < count; i++)
            {
                if (_moneyPrefabs.Count < 10)
                {
                    var item = Instantiate(resourceItem, initSpawnPoint.position + Vector3.right * Random.Range(-2.5f, 2.5f), Quaternion.identity);
                    _moneyPrefabs.Add(item);
                }
            }

            _currentCount += count;

            if(!_moneyPrefabs.Any()) return;
            
            var prefabsCount = _moneyPrefabs.Count;
            var avgCost = _currentCount / prefabsCount;
            var extraCost = _currentCount % prefabsCount;

            _moneyPrefabs.ForEach(money => money.OnCollection -= TakeMoney);

            foreach (var resource in _moneyPrefabs)
            {
                resource.Initialize(_gameResourceService, Enumerable.Repeat((EResourceType.CoinGold, avgCost), 1));
            }

            _moneyPrefabs[^1].Initialize(_gameResourceService, Enumerable.Repeat((EResourceType.CoinGold, avgCost + extraCost), 1));

            _moneyPrefabs.ForEach(money => money.OnCollection += TakeMoney);

            if (save)
            {
                _gameSave.SetData(SaveConstants.DollarsInShelf, _currentCount);
            }
        }

        private void TakeMoney(CollectingResourceObject resourceObject)
        {
            _moneyPrefabs.Remove(resourceObject);
            _currentCount -= resourceObject.Count;
            
            _gameSave.SetData(SaveConstants.DollarsInShelf, _currentCount);
        }

        private void Start()
        {
            var savedValue = _gameSave.GetData(SaveConstants.DollarsInShelf, 0);
            AddMoney(savedValue, false);
        }
    }
}
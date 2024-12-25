using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.BattleMode;
using Game.Scripts.DamageEffector;
using Game.Scripts.DamageEffector.Data;
using Game.Scripts.DamageEffector.Interface;
using Game.Scripts.Days;
using Game.Scripts.NPC.Fish;
using Game.Scripts.NPC.Fish.Configs;
using Game.Scripts.NPC.Fish.Configs.Structs;
using Game.Scripts.NPC.Fish.FishStates.Passive;
using Game.Scripts.NPC.Fish.Systems;
using Game.Scripts.Player;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Pool;
using Game.Scripts.ResourceController.Interfaces;
using GTap.Analytics;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.NPC.Spawners
{
    public class FishSpawner : MonoBehaviour
    {
        public event Action<BaseFish> OnDeadFish; 
        
        [SerializeField, FoldoutGroup("Components")]
        private FishHabitat _habitat;

        [SerializeField] private float delay;
        [SerializeField] private FishInBiomes _fishInBiome;
        [SerializeField] private AnimationCurve _curveBoostMove;
        
        private int _fishCollected;
        private int _currentDay;
        private float _currentTime;
        private Transform _fishParent;
        private Transform _playerTransform;
        private SerializableFishPerDay _currentFishPerDay;
        private List<BaseFish> _spawnedFish = new();
        private DamageEffectSpawner _effectSpawner;
        private IResourceSpawner _spawner;
        private ISpawnEffectService _gameEffectSpawner;
        private IPlayerUpgradeSettings _upgradeSettings;
        private IGameResourceService _gameResource;
        private IDayService _dayController;
        private IBattleService _battleService;
        private PoolObjects<BaseFish> _fishPool;
        private IPlayerService _playerService;

        [Inject]
        private void Construct(IDayService dayController, DamageEffectSpawner effectSpawner, IResourceSpawner spawner,
            ISpawnEffectService gameEffectSpawner, IPlayerService playerService, IGameResourceService resourceService, IBattleService battleService)
        {
            _battleService = battleService;
            _playerService = playerService;
            _upgradeSettings = _playerService.UpgradeSettings;
            _playerService.AddListener(ValidatePlayer);
            _dayController = dayController;
            _gameResource = resourceService;
            _effectSpawner = effectSpawner;
            _spawner = spawner;
            _gameEffectSpawner = gameEffectSpawner;

            _upgradeSettings.OnUpgrade += UpgradeSettingsOnOnUpgrade;
            _dayController.OnDayStart += ValidateDay;
            _dayController.OnDayComplete += PerformFishAnalytics;
            _battleService.OnChangeState += ValidateBattle;
        }

        public List<BaseFish> GetFishOnRadius(Vector3 position, int level = 1)
        {
            if (_spawnedFish == null)
            {
                return null;
            }

            var result = new List<BaseFish>();
            foreach (var fish in _spawnedFish)
            {
                result.Add(GetDistanceFish(result, position, level));
            }

            return result;
        }

        private BaseFish GetDistanceFish(List<BaseFish> contains, Vector3 position, int level)
        {
            BaseFish result = null;
            var distance = float.MaxValue;
            foreach (var fish in _spawnedFish)
            {
                if (fish.HaveLife == false)
                {
                    continue;
                }
                
                if (fish.Level != level)
                {
                    continue;
                }
                
                if (contains.Contains(fish))
                {
                    continue;
                }
                
                var dist = Vector3.Distance(fish.transform.position, position);
                if (dist < distance)
                {
                    result = fish;
                    distance = dist;
                }
            }

            return result;
        }

        private void ValidateDay(int dayId)
        {
            PerformSpawn(dayId);
        }

        private void PerformFishAnalytics(int completedDay)
        {
            GtapAnalytics.ResourceCollected(completedDay, _dayController.AttemptsCount, EResourceCollected.Fish, _fishCollected);

            _fishCollected = 0;
        }

        private void ValidatePlayer(EPlayerStates state, EntityPlayer player)
        {
            _playerTransform = player.transform;
        }

        private void ValidateBattle(BattleState state)
        {
            switch (state)
            {
                case BattleState.СutScene:
                    foreach (var baseFish in _spawnedFish)
                    {
                        baseFish.ComponentsFish.MoveToTarget.SetTarget(baseFish.transform, Vector3.down);
                    }

                    break;
                case BattleState.Fight:
                    Clear();
                    break;
            }
        }

        private void PerformSpawn(int dayId)
        {
            var resourcesPerDay = _fishInBiome.FishPerDays.FirstOrDefault(pair => pair.DayId == dayId);

            if (resourcesPerDay.FishPerLevels == null)
            {
                _currentFishPerDay = _fishInBiome.FishPerDays[^1];
            }
            else
            {
                _currentFishPerDay = resourcesPerDay;
            }

            Clear();

            Spawn(_currentFishPerDay);
        }

        private void Spawn(SerializableFishPerDay fishPerDay)
        {
            var fishData = _fishInBiome.FishConfig;
            foreach (var depthLevel in fishPerDay.FishPerLevels)
            {
                for (var i = 0; i < depthLevel.FishCount.Length; i++)
                {
                    var element = depthLevel.FishCount[i];
                    var level = element.FishLevel;
                    var spawnedCount = _spawnedFish.Count(fish => fish.Level == level);
                    var count = element.Count - spawnedCount;
                    var data = _fishInBiome.FishConfig.FishData.First(d => d.Level == level);
                    var tempNewFishes = Enumerable.Range(0, count).Select(x =>
                    {
                        var fish = _fishPool.GetFree();
                        var targetPosition = GetXPositionOnSide(Random.Range(0, 10) > 4);

                        var spawnPosition = new Vector3(targetPosition, Random.Range(depthLevel.yMax, depthLevel.yMin), 0);
                        fish.transform.position = spawnPosition;
                        fish.YSpawnPosition = spawnPosition.y;
                        fish.Initialize(data, _habitat, depthLevel.yMin, depthLevel.yMax);
                        fish.transform.SetParent(_fishParent);
                        fish.gameObject.SetActive(true);

                        fish.Construct(_effectSpawner);
                        if (fish.StateMachine == null)
                        {
                            var stateMachine = CreateStateMachine(fish);
                            fish.InitState(stateMachine);
                        }

                        fish.OnDead += PerformOnFishDead;
                        fish.ComponentsFish.Bar.SetStateLock(_upgradeSettings.GetLevel(EPlayerUpgradeType.FishLevel) >= fish.Data.Level ? FishLockState.UnLock : FishLockState.Lock);
                        fish.SetBarrierHeight(-_upgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxDepth));
                        SetDefaultState(fish.StateMachine, fish);
                        return fish;
                    });
                    
                    _spawnedFish.AddRange(tempNewFishes);
                }
            }
        }

        private float GetXPositionOnSide(bool isRight)
        {
            var position = _playerTransform.position.x;
            var rnd = Random.Range(10f, 30f);
            position += isRight ? rnd : -rnd;

            if (Mathf.Abs(position) > 50)
            {
                position *= -1;
            }

            position = Mathf.Clamp(position,-50, 50);
            return position;
        }

        private void Clear()
        {
            var fishes = _spawnedFish.ToArray();
            for (var i = 0; i < fishes.Length; i++)
            {
                fishes[i].OnDead -= PerformOnFishDead;
                fishes[i].gameObject.SetActive(false);
            }

            _spawnedFish.Clear();
        }

        private void UpgradeSettingsOnOnUpgrade(EPlayerUpgradeType type)
        {
            if (type is not EPlayerUpgradeType.FishLevel and not EPlayerUpgradeType.MaxDepth )
            {
                return;
            }

            foreach (var fish in _spawnedFish)
            {
                fish.ComponentsFish.Bar.SetStateLock(_upgradeSettings.GetLevel(EPlayerUpgradeType.FishLevel) >= fish.Data.Level ? FishLockState.UnLock : FishLockState.Lock);
                fish.SetBarrierHeight(-_upgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxDepth));
            }
        }

        private void OnDisable()
        {
            foreach (var fish in _spawnedFish)
            {
                fish.OnDead -= PerformOnFishDead;
            }

            if (_upgradeSettings != null)
            {
                _upgradeSettings.OnUpgrade -= UpgradeSettingsOnOnUpgrade;
            }
        }

        private void PerformOnFishDead(BaseFish baseFish)
        {
            OnDeadFish?.Invoke(baseFish);
            _fishCollected += 1;
            baseFish.OnDead -= PerformOnFishDead;
            _gameEffectSpawner.SpawnEffect(EffectType.DeadFish, baseFish.transform.position);
            _spawnedFish.Remove(baseFish);
            foreach (var item in baseFish.Data.DropItemsList)
            {
                _gameResource.Add(item.ResourceType, item.Count);
            }
        }

        private EntityStateMachine CreateStateMachine(BaseFish baseFish)
        {
            var stateMachine = new EntityStateMachine();
            EntityState stateIdle = new FishIdle(stateMachine, baseFish);
            EntityState stateMove = new FishMove(stateMachine, baseFish);
            EntityState stateEnterTarget = new FishMoveAway(stateMachine, baseFish, _curveBoostMove);
            stateMachine.AddState(stateIdle);
            stateMachine.AddState(stateMove);
            stateMachine.AddState(stateEnterTarget);
            return stateMachine;
        }

        private void SetDefaultState(EntityStateMachine stateMachine, BaseFish baseFish)
        {
            stateMachine.SetState<FishMove>();
            baseFish.enabled = true;
        }

        private void Update()
        {
            if (_battleService.CurrentState == BattleState.Idle)
            {
                _currentTime -= Time.deltaTime;

                if (_currentTime < 0)
                {
                    _currentTime = delay;
                    Spawn(_currentFishPerDay);
                }
            }
        }

        private void Awake()
        {
            _fishParent = new GameObject("FishParent").transform;
            
            var fishPrefab = _fishInBiome.FishConfig.FishPrefab;
            _fishPool = new PoolObjects<BaseFish>(fishPrefab, _fishParent, 10);
        }

        private void OnDestroy()
        {
            _upgradeSettings.OnUpgrade -= UpgradeSettingsOnOnUpgrade;
            _dayController.OnDayStart -= ValidateDay;
            _dayController.OnDayComplete -= PerformFishAnalytics;
            _battleService.OnChangeState -= ValidateBattle;
        }
    }
}
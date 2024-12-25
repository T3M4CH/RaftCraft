using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.CollectingResources;
using Game.Scripts.DamageEffector;
using Game.Scripts.DamageEffector.Interface;
using Game.Scripts.Extension;
using Game.Scripts.NPC;
using Game.Scripts.NPC.Fish;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.UI.WindowManager.Windows;
using UnityEngine;
using Random = UnityEngine.Random;

//TODO: НУЖНО ВСЕ СДЕЛАТЬ КРАСИВО И ГРАМОТНО
public class EnemyRaft : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private float _delayBetweenSpawn = 1f;
    [SerializeField] private float _speedMove = 5f;
    [SerializeField] private float _offsetHalf = 5f;
    [SerializeField] private Rope _rope;
    [SerializeField] private ArrowEnemy _arrowEnemy;

    private ArrowEnemy _arrow;
    private HumanBase[] _humanQueue;
    private DropItems[] _dropItems;
    private WaitForSeconds _cooldown;
    private PoolManager _poolManager;
    private NPCNavigation _npcNavigation;
    private IResourceSpawner _resourceSpawner;
    private Sequence _sequence;
    private List<HumanBase> _activeHumans = new List<HumanBase>();
    private List<CollectingResourceObject> _uncollectedResource = new List<CollectingResourceObject>();
    private BattleStatistics _battleStatistics;
    private DamageEffectSpawner _damageEffectSpawner;
    private ISpawnEffectService _spawnEffectService;
    private int _currentSpawnIndex;
    private bool _isLeftSide;
    private GrabPoint _grabPoint;
    private WindowBattle _windowBattle;

    public void Init(NPCNavigation npcNavigation, WindowBattle windowBattle, IResourceSpawner resourceSpawner, HumanBase[] humanQueue,
        DropItems[] dropItems, DamageEffectSpawner damageEffectSpawner, ISpawnEffectService spawnEffectService, BattleStatistics battleStatistics)
    {
        _battleStatistics = battleStatistics;
        _windowBattle = windowBattle;
        _spawnEffectService = spawnEffectService;
        _damageEffectSpawner = damageEffectSpawner;
        _humanQueue = humanQueue;
        _dropItems = dropItems;
        _resourceSpawner = resourceSpawner;
        _npcNavigation = npcNavigation;
        _poolManager = new PoolManager();
        _cooldown = new WaitForSeconds(_delayBetweenSpawn);

        StartCoroutine(MoveToPlayerRaft());
        InitialSpawn();

        battleStatistics.OnPlayerWin += OnPlayerWin;
    }

    private void InitialSpawn()
    {
        foreach (var point in _spawnPoints)
        {
            SpawnHuman(_humanQueue[_currentSpawnIndex], point.position, false);

            if (_humanQueue.Length == _currentSpawnIndex) break;
        }
        
        _arrow = Instantiate(_arrowEnemy, _windowBattle.transform, false);
        _arrow.Setup(this);
    }

    public Transform GetArrowTarget()
    {
        if (_activeHumans.Count == 0)
        {
            Destroy(_arrow.gameObject);
            return null;
        }
        
        var minDistance = Mathf.Infinity;
        var target = _activeHumans.Peek().transform;
        for (int i = 0; i < _activeHumans.Count; i++)
        {
            var distance = Vector3.Distance(_activeHumans[i].Position, _npcNavigation.Player.Position);

            if (distance < minDistance)
            {
                minDistance = distance;
                target = _activeHumans[i].transform;
            }
        }
        return target;
    }
    
    private IEnumerator MoveToPlayerRaft()
    {
        _isLeftSide = transform.position.x < _npcNavigation.Player.transform.position.x;
        var target = _isLeftSide
            ? _npcNavigation.RaftConstructor.GetLeftRaftPosition() - new Vector3(_offsetHalf, 0f, 0f)
            : _npcNavigation.RaftConstructor.GetRightRaftPosition() + new Vector3(_offsetHalf, 0f, 0f);

        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _speedMove);
            yield return null;
        }

        foreach (var human in _activeHumans)
        {
            human.Command(new Move());
        }
        
        _npcNavigation.RaftConstructor.OffsetHorizontal += _isLeftSide ? new Vector2(10f, 0f) : new Vector2(0f, 10f);

        PerformGrab();

        StartCoroutine(SpawnQueue());
    }

    private void PerformGrab()
    {
        if (!_rope) return;
        
        _grabPoint = _npcNavigation.GetGrabPoint(_isLeftSide);
        
        if(_grabPoint.HaveInteraction)
        {
            _rope.GrabToPoint(_grabPoint.Position);
        }
    }
    
    private IEnumerator SpawnQueue()
    {
        while (_currentSpawnIndex < _humanQueue.Length)
        {
            yield return _cooldown;

            var pointNumber = _currentSpawnIndex % _spawnPoints.Length;
            var prefab = _humanQueue[_currentSpawnIndex];
            var position = _spawnPoints[pointNumber].position;

            if (prefab is StormTrooper && _rope)
            {
                position = _rope.transform.position + new Vector3(0f, -4f, 0f);
            }
            
            SpawnHuman(prefab, position, true);
        }
    }

    private void SpawnHuman(HumanBase humanBase, Vector3 position, bool isCommand)
    {
        var rotate = new Vector3(0f, _isLeftSide ? Random.Range(60f, 120f) : Random.Range(-60f, -120f), 0f);

        var human = _poolManager.GetPool(humanBase, position, Quaternion.Euler(rotate));
        human.transform.SetParent(transform);
        human.Init(_npcNavigation, _damageEffectSpawner, _spawnEffectService);
        human.OnDeathCallback += OnDeathHuman;

        _currentSpawnIndex++;
        _activeHumans.Add(human);

        if (isCommand)
            SelectCommand(human);
    }

    private void SelectCommand(HumanBase human)
    {
        switch (human)
        {
            case StormTrooper:
                if(_rope && _grabPoint.HaveInteraction)
                {
                    human.Command(new MoveRope()
                    {
                        Target = _grabPoint.Position + new Vector3(0f, -4f, 0f),
                    });
                }
                else
                {
                    human.Command(new Move());
                }
                break;
            case MeleeHuman:
                human.Command(new Move());
                break;
            default:
                human.Command(new Move());
                break;
        }
    }
    
    private void OnDeathHuman(HumanBase humanBase, bool isEnterState)
    {
        if (isEnterState)
        {
            _battleStatistics.UpdateProgress();
            return;
        }
        
        foreach (var item in humanBase.Drop)
        {
            var res = _resourceSpawner.SpawnItem(humanBase.transform.position + Vector3.up, resources: (item.ResourceType, item.Count));
            _uncollectedResource.Add(res);
            res.IsAbsorbent = true;

            res.OnCollection += ItemOnOnCollection;
        }

        humanBase.OnDeathCallback -= OnDeathHuman;
        _activeHumans.Remove(humanBase);
        _poolManager.SetPool(humanBase);

        _battleStatistics.DestructionCheck();
    }

    private void ResourceSpawn()
    {
        var num = 0;
        foreach (var item in _dropItems)
        {
            var point = _spawnPoints[num].position;
            point.y -= Random.Range(2f, 4f);
            _resourceSpawner.SpawnItem(point, true, resources: (item.ResourceType, item.Count));
            num = (num + 1) % _spawnPoints.Length;
        }

        foreach (var item in _uncollectedResource)
        {
            item.CollideArea(_npcNavigation.Player.transform);
        }
    }

    private void OnPlayerWin(bool value)
    {
        if(value == false)
        {
            if(_arrow)
                Destroy(_arrow.gameObject);
            
            StopAllCoroutines();

            foreach (var human in _activeHumans)
                _poolManager.SetPool(human);

            _activeHumans.Clear();
        }
    }

    public void DestroyRaft(bool isAnim, Action callback = null)
    {
        if (isAnim == false)
        {
            Destroy(gameObject);
            return;
        }
        
        if (_boxCollider)
        {
            _boxCollider.enabled = false;
        }

        if (_rope)
        {
            _rope.gameObject.SetActive(false);
        }

        var feedback = GetComponent<RaftFeedback>();
        _sequence = DOTween.Sequence()
            .AppendCallback(() =>
            {
                feedback.Crumble();
                ResourceSpawn();
                callback?.Invoke();
            })
            .AppendInterval(2f)
            .AppendCallback(() =>
            {
                feedback.DestroyParts(this);
            });
    }

    private void ItemOnOnCollection(CollectingResourceObject resource)
    {
        resource.OnCollection -= ItemOnOnCollection;
        _uncollectedResource.Remove(resource);
    }

    private void OnDestroy()
    {
        _battleStatistics.OnPlayerWin -= OnPlayerWin;

        _sequence.Kill();

        for (int i = 0; i < _activeHumans.Count; i++)
            _activeHumans[i].OnDeathCallback -= OnDeathHuman;

        foreach (var resource in _uncollectedResource)
            resource.OnCollection -= ItemOnOnCollection;
    }
}
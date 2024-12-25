using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Prefabs.NPC.Vendors;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Pool;
using Game.Scripts.Raft.BuildSystem;
using Game.Scripts.Saves;
using GTapSoundManager.SoundManager;
using JetBrains.Annotations;
using Reflex.Attributes;
using Sirenix.Utilities;
using UnityEngine;

public class MonoNetController : MonoBehaviour
{
    [SerializeField] private int targetFishCount;
    [SerializeField] private float moveTime;
    [SerializeField] private SoundAsset soundAsset;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private Transform center;
    [SerializeField] private TileBuild tileBuild;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private MonoCaughtFish caughtFishPrefab;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private MonoFisher monoFisher;

    private float _radius;
    private GameSave _gameSave;
    private PoolObjects<MonoCaughtFish> _caughtFishPool;

    private Transform _playerTransform;
    public List<MonoCaughtFish> Fishes = new();
    private IPlayerService _playerService;

    [Inject]
    private void Construct(GameSave gameSave, IPlayerService playerService)
    {
        _playerService = playerService;
        _playerService.AddListener(ValidatePlayer);
        _gameSave = gameSave;
    }

    public void AddFish(int level, float pitch, bool isJustSpawned = false)
    {
        var fish = _caughtFishPool.GetFree();
        fish.enabled = false;
        fish.SetLevel(level);
        fish.gameObject.SetActive(true);
        
        if (_playerTransform && !isJustSpawned)
        {
            var position = _playerTransform.position;
            position.y += 1;

            fish.transform.position = position;
            //var path = new[] { position, pickPoint.position, center.position };
            fish.transform.localScale = Vector3.one;
            StartCoroutine(MoveFish(fish, center.position, moveTime, pitch));
            //fish.transform.DOPath(path, moveTime, PathType.CatmullRom).SetLookAt(0.01f).SetLink(fish.gameObject);
            //fish.transform.DOScale(1, 0.2f).SetEase(Ease.InBounce).From(0).OnKill(() => { fish.enabled = true; }).SetLink(fish.gameObject);
            return;
        }
        else
        {
            fish.transform.position = center.position;
            fish.transform.localScale = Vector3.one;
            fish.enabled = true;
            Fishes.Add(fish);
        }
        
        ValidateSizes();
    }

    public MonoCaughtFish TakeFish()
    {
        if (Fishes.Count <= 0) return null;

        var fish = Fishes[0];
        fish.gameObject.SetActive(false);

        Fishes.Remove(fish);

        ValidateSizes();
        return fish;
    }

    private void ValidatePlayer(EPlayerStates state, EntityPlayer player)
    {
        _playerTransform = player.transform;
    }

    private void ValidateSizes()
    {
        var ratio = (float)Fishes.Count / targetFishCount;

        var position = center.localPosition;
        position.y = Mathf.Lerp(0.2f, 1.87f, ratio);
        center.localPosition = position;

        sphereCollider.radius = Mathf.Lerp(1, 3, ratio);
        skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Lerp(0, 40f, ratio));

        foreach (var fish in Fishes)
        {
            fish.UpdateRadius(sphereCollider.radius, center.position);
        }

        var fishLevels = Fishes.Select(fish => fish.Level - 1).ToArray();
        _gameSave.SetData(SaveConstants.FishInNet, fishLevels);
    }

    private void ChangeState(BuildState state, TileBuild tile)
    {
        sphereCollider.enabled = state == BuildState.UnLock;
    }

    private void Start()
    {
        tileBuild.OnStateChanged += ChangeState;
        _caughtFishPool = new PoolObjects<MonoCaughtFish>(caughtFishPrefab, transform.parent, 10);

        var fishes = _gameSave.GetData(SaveConstants.FishInNet, new int[] { });

        for (var i = 0; i < fishes.Length; i++)
        {
            AddFish(fishes[i], 0, true);
        }
    }

    private IEnumerator MoveFish(MonoCaughtFish caughtFish, Vector3 target, float estimatedTime, float pitch)
    {
        var time = 0f;
        var fish = caughtFish.transform;
        var startPosition = fish.position;
        while (time < estimatedTime)
        {
            var ratio = time / estimatedTime;
            time += Time.deltaTime * speedCurve.Evaluate(ratio);

            var position = fish.position;
            position = Vector3.Lerp(startPosition, target, ratio);
            position.y = heightCurve.Evaluate(ratio);

            fish.LookAt(position);
            fish.position = position;

            yield return null;
        }

        soundAsset.Play(pitch);

        caughtFish.enabled = true;
        Fishes.Add(caughtFish);

        ValidateSizes();

        monoFisher.AddTime(caughtFish.Level - 1);
        monoFisher.PerformNotify(null);
    }
}
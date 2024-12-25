using System;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.CollectingResources;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.Raft.BuildSystem;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player;
using Game.Scripts.Saves;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Game.Scripts.ResourceController;
using Game.Scripts.UI.WindowManager.Windows;
using UnityEngine.UI;

public class MonoTutorialOne : MonoTutorialBase
{
    [SerializeField] private TutorialBuilt buildInteractor;
    [SerializeField] private MonoMovingJoystick movingJoystick;
    [SerializeField] private CollectionTutorialResource[] collectables;
    [SerializeField] private RectTransform _cursor;

    private bool _isLogsCollected;
    private int _destroyedLogsCount;
    private GameSave _gameSave;
    private GameObject _raft;
    private TileBuild _tileBuild;
    private TileBuild _tileBuildNext;
    private WindowManager _windowManager;
    private IPlayerService _playerSpawner;
    private EntityPlayer _entityPlayer;
    private IResourceService _resourceService;
    private IGameResourceService _gameResourceService;
    private Sequence _sequence;
    private Button _fightButton;
    private RectTransform _fightButtonRect;
    private Vector3 _fightButtonScale;
    private Vector3 _cursorStartPosition;
    private bool _isBattleButtonPart;

    public override void Initialize(GameSave gameSave, IPlayerService playerSpawner, WindowManager windowManager, object extraParams = null)
    {
        // try
        // {
        //     var type = extraParams.GetType();
        //     _gameResourceService = (IGameResourceService)type.GetProperty("_gameResourceService")?.GetValue(extraParams);
        //     _resourceService = (IResourceService)type.GetProperty("_resourceService")?.GetValue(extraParams);
        //
        //     if (_gameResourceService == null || _resourceService == null)
        //     {
        //         throw new Exception("Fail cast");
        //     }
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError(e);
        // }
        //
        // _tileBuildNext = FindObjectsOfType<TileBuild>().Where(tile => tile.name == "Tile_1").OrderBy(x => x.Cost).First();
        // _tileBuild = FindObjectsOfType<TileBuild>().Where(tile => tile.name == "Tile_2").OrderByDescending(x => x.Cost).First();
        // _tileBuild.SetState(BuildState.VisibleOnBuild);
        //
        // _gameSave = gameSave;
        // _playerSpawner = playerSpawner;
        // _windowManager = windowManager;
        //
        // _fightButton = _windowManager.GetWindow<WindowGame>().FightButton;
        // _fightButtonRect = _fightButton.GetComponent<RectTransform>();
        // _fightButtonScale = _fightButtonRect.localScale;
        // _fightButton.onClick.AddListener(Complete);
        // _cursorStartPosition = _cursor.localPosition;
        //
        // _playerSpawner.AddListener(ValidatePlayer);
        //
        // InitializeLogs(_gameResourceService);
        // ShowJoystick();
    }

    // private void InitializeLogs(IGameResourceService resourceService)
    // {
    //     foreach (var collectable in collectables)
    //     {
    //         collectable.Construct(resourceService);
    //         collectable.InitializeCallback(() =>
    //         {
    //             _destroyedLogsCount += 1;
    //
    //             if (_destroyedLogsCount >= collectables.Length)
    //             {
    //                 _isLogsCollected = true;
    //                 ShowTileBuild();
    //             }
    //         });
    //     }
    // }
    //
    // private void ValidatePlayer(EPlayerStates playerState, EntityPlayer entityPlayer)
    // {
    //     _entityPlayer = entityPlayer;
    //     if (playerState == EPlayerStates.PlayerInWater)
    //     {
    //         if (!_isLogsCollected)
    //         {
    //             for (var i = 0; i < collectables.Length; i++)
    //             {
    //                 collectables[i].SetActiveArrow(true);
    //             }
    //         }
    //     }
    //
    //     if (playerState == EPlayerStates.PlayerInRaft)
    //     {
    //         _isBattleButtonPart = true;
    //         PerformCursor();
    //     }
    //     else
    //     {
    //         if (_isBattleButtonPart)
    //         {
    //             _cursor.gameObject.SetActive(false);
    //         }
    //     }
    // }
    //
    // private void PerformCursor()
    // {
    //     _sequence.Kill();
    //     _fightButtonRect.DOKill();
    //
    //     ShowBattleButton(true);
    //     _cursor.gameObject.SetActive(true);
    //
    //     _fightButtonRect.DOScale(1.1f, 0.5f).From(_fightButtonScale).SetEase(Ease.InQuad).SetLoops(int.MaxValue, LoopType.Yoyo);
    //     _sequence = DOTween.Sequence();
    //     _sequence.Append(_cursor.DOMove(_fightButtonRect.position + new Vector3(-25, -5, 0), 1).From(_cursorStartPosition));
    //     _sequence.Append(_cursor.DOScale(0.95f, 1).SetLoops(int.MaxValue, LoopType.Yoyo)).SetLoops(int.MaxValue, LoopType.Yoyo);
    // }
    //
    // private void ShowTileBuild()
    // {
    //     buildInteractor.Initialize(_resourceService);
    //     _tileBuild.SetState(BuildState.VisibleOnBuild);
    //     _tileBuild.OnBuyTile += ShowRaft;
    // }
    //
    // private void ShowRaft()
    // {
    //     _tileBuild.OnBuyTile -= ShowRaft;
    //     _tileBuildNext.SetState(BuildState.Lock);
    //     _entityPlayer.Animator.SetBool("SpawnedInWater", false);
    // }
    //
    // private void ShowJoystick()
    // {
    //     movingJoystick.Construct(_windowManager);
    //     movingJoystick.gameObject.SetActive(true);
    //     movingJoystick.OnMoving += ShowArrows;
    // }
    //
    // private void ShowArrows()
    // {
    //     movingJoystick.OnMoving -= ShowArrows;
    //
    //     foreach (var collectionTutorialResource in collectables)
    //     {
    //         collectionTutorialResource.SetActiveArrow(true);
    //     }
    // }
    //
    // protected override void Complete()
    // {
    //     _fightButton.onClick.RemoveListener(Complete);
    //     _gameSave.SetData(SaveConstants.TutorialOne, true);
    //     _tileBuildNext.SetState(BuildState.VisibleOnBuild);
    //     _tileBuildNext.SetStateVisibleBuild(false);
    //     _tileBuild.gameObject.SetActive(true);
    //     _tileBuild.OnBuyTile.Invoke();
    //
    //     Destroy(gameObject);
    // }
    //
    // private void OnDestroy()
    // {
    //     if (_fightButtonRect)
    //     {
    //         _fightButtonRect.DOKill();
    //         _fightButtonRect.localScale = _fightButtonScale;
    //     }
    //
    //     _sequence.Kill();
    //     if (_playerSpawner != null)
    //     {
    //         _playerSpawner.RemoveListener(ValidatePlayer);
    //     }
    // }
}
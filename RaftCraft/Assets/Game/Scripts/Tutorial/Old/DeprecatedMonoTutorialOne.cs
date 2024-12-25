using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.BattleMode;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Player.Spawners;
using Game.Scripts.CollectingResources;
using Game.Scripts.Extension;
using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.Player;
using Game.Scripts.Raft.BuildSystem;
using Game.Scripts.Raft.CustomBuilder;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using Game.Scripts.Tutorial.Old;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowManager.Windows;
using GTap.Analytics;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine;

public class DeprecatedMonoTutorialOne : MonoTutorialBase
{
    [SerializeField] private BubbleCollectable bubbleCollectable;
    [SerializeField] private MonoArrowCanvas[] edgeArrows;
    [SerializeField] private MonoArrowCanvas[] climbArrows;
    [SerializeField] private MonoArrowCanvas resourcesText;
    [SerializeField] private CollectionTutorialResource[] collectablesFazeOne;
    [SerializeField] private CollectionTutorialResource[] collectablesFazeTwo;
    [SerializeField] private CollectionTutorialResource[] collectablesFazeThree;
    [SerializeField] private RectTransform _cursor;
    [SerializeField] private Transform effectPosition;
    [SerializeField] private GameObject _cursorParticle;
    [SerializeField] private LineRendererMove _prefabLine;
    [SerializeField] private MonoMovingJoystick tutorialJoystick;
    
    private int _countCollectedFazeOne;
    private int _countCollectedFazeTwo;
    private int _countCollectedFazeThree;
    
    private bool _isRaftUnlocked;
    private bool _isLogsCollected;
    private bool _isBattleButtonPart;
    private bool _isWaterTutorialComplete;
    private Vector3 _cursorStartPosition;
    private Vector3 _fightButtonScale;
    private Vector3 _cursorParticleScale;
    private CustomBuilderTile _tileBuild;
    private TileBuild _tileBuildComplete;
    private RectTransform _fightButtonRect;
    private Sequence _sequence;
    private GameSave _gameSave;
    private WindowManager _windowManager;
    private EntityStateMachine _entityStateMachine;
    private IPlayerService _playerSpawner;

    private LineRendererMove _lineRenderer;
    private Transform _current;
    private Transform _player;
    
    [SerializeField,ReadOnly]
    private int _faze;

    private int Faze
    {
        get => _faze;
        set
        {
            switch (_faze)
            {
                case 0:
                    resourcesText.gameObject.SetActive(false);
                    break; 
                case 1:
                    foreach (var resource in collectablesFazeOne)
                    {
                        resource.OnCollection -= OnCollectedFazeOne;
                    }

                    break;
                case 2:
                    _tileBuild.OnStartBuilding -= TileBuildOnOnStartBuilding;
                    _tileBuild.OnBuildLeft -= TileBuildOnOnBuildLeft;
                    resourcesText.gameObject.SetActive(false);
                    break;
                case 3:
                    foreach (var resource in collectablesFazeTwo)
                    {
                        resource.OnCollection -= OnCollectedFazeTwo;
                    }
                    break;
                case 4:
                    _tileBuild.OnStartBuilding -= TileBuildOnOnStartBuilding;
                    _tileBuild.OnBuildRight -= TileBuildOnOnBuildRight;
                    resourcesText.gameObject.SetActive(false);
                    break;
                case 5:
                    foreach (var resource in collectablesFazeThree)
                    {
                        resource.OnCollection -= OnCollectedFazeThree;
                    }
                    break;
                case 6:
                    _current = null;
                    ChangeLine();
                    _tileBuildComplete.OnStartBuilding -= OnStartBuild;
                    break;
                case 7 :
                    _tileBuildComplete.OnBuyTile -= OnBuyTile;
                    break;
                case 8:
                    break;
            }

            _faze = Mathf.Clamp(value, 0, int.MaxValue);
            switch (_faze)
            {
                case 1:
                    _lineRenderer = Instantiate(_prefabLine, transform);
                    _lineRenderer.CountPoint = 2;
                    _windowManager.SetNameTutorial("Collect Wood");
                    InitializeLogs(collectablesFazeOne, _resourceService);
                    foreach (var resource in collectablesFazeOne)
                    {
                        resource.gameObject.SetActive(true);
                        resource.OnCollection += OnCollectedFazeOne;
                    }
                    _lineRenderer.SetStateLine(true);
                    break;
                case 2:
                    _tileBuild.StartLogic();
                    _windowManager.SetNameTutorial("Build Raft");
                    _current = _tileBuild.TransformUI;
                    _lineRenderer.SetStateLine(true);
                    resourcesText.transform.position = _tileBuild.PositionUI;
                    resourcesText.gameObject.SetActive(true);
                    _tileBuild.OnStartBuilding += TileBuildOnOnStartBuilding;
                    _tileBuild.OnBuildLeft += TileBuildOnOnBuildLeft;
                    break;
                case 3:
                    _windowManager.SetNameTutorial("Collect Wood");
                    InitializeLogs(collectablesFazeTwo, _resourceService);
                    foreach (var resource in collectablesFazeTwo)
                    {
                        resource.gameObject.SetActive(true);
                        resource.OnCollection += OnCollectedFazeTwo;
                    }
                    _lineRenderer.SetStateLine(true);
                    break;
                case 4:
                    _windowManager.SetNameTutorial("Build Raft");
                    _tileBuild.OnStartBuilding += TileBuildOnOnStartBuilding;
                    _tileBuild.StartBuildRight();
                    _current = _tileBuild.TransformUI;
                    _lineRenderer.SetStateLine(true);
                    resourcesText.transform.position = _tileBuild.PositionUI;
                    resourcesText.gameObject.SetActive(true);
                    _tileBuild.OnBuildRight += TileBuildOnOnBuildRight;
                    break;
                case 5:
                    var posArrow = edgeArrows[0].transform.position;
                    posArrow.x += 4.5f;
                    edgeArrows[0].transform.position = posArrow;
                    InitializeLogs(collectablesFazeThree, _resourceService);
                    foreach (var resource in collectablesFazeThree)
                    {
                        resource.gameObject.SetActive(true);
                        resource.OnCollection += OnCollectedFazeThree;
                    }
                    _lineRenderer.SetStateLine(true);
                    break;
                case 6:
                    _current = _tileBuildComplete.UITransform;
                    _lineRenderer.SetStateLine(true);
                    _tileBuildComplete.OnStartBuilding += OnStartBuild;
                    break;
                case 7:
                    _tileBuildComplete.OnBuyTile += OnBuyTile;
                    break;
                case 8:
                    _battleService.OnChangeState += BattleServiceOnOnChangeState;
                    _windowManager.SetNameTutorial("Start Battle");
                    ShowCursor();
                    break;
                case 9:
                    break;
                case 10:
                    _tileBuildComplete.SetStateVisibleBuild(true);
                    _battleService.OnChangeState -= BattleServiceOnOnChangeState;
                    Complete();
                    break;
            }
        }
    }

    private void OnBuyTile()
    {
        Faze = 8;
    }

    private void OnStartBuild()
    {
        Faze = 7;
    }


    private void BattleServiceOnOnChangeState(BattleState state)
    {
        switch (state)
        {
            case BattleState.СutScene:
                _windowManager.SetNameTutorial("");
                _cursor.gameObject.SetActive(false);
                _cursorParticle.gameObject.SetActive(false);
                break;
            case BattleState.Fight:
                _cursorParticle.gameObject.SetActive(false);
                Faze = 9;
                break;
            case BattleState.Result:
                Faze = 10;
                break;
            case BattleState.Idle:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void TileBuildOnOnStartBuilding()
    {
        _current = null;
        _lineRenderer.SetStateLine(false);
        _tileBuild.OnStartBuilding -= TileBuildOnOnStartBuilding;
        resourcesText.gameObject.SetActive(false);
    }

    private void TileBuildOnOnBuildRight()
    {
        Faze = 8;
    }

    private void TileBuildOnOnBuildLeft()
    {
        Faze = 3;
    }

    private IGameResourceService _resourceService;
    private IBattleService _battleService;
    private List<CollectingResourceObject> _collectingResource = new List<CollectingResourceObject>();
    
    public override void Initialize(GameSave gameSave, IPlayerService playerSpawner, WindowManager windowManager, object extraParams = null)
    {
        var type = extraParams.GetType();
        _resourceService = (IGameResourceService)type.GetProperty("_gameResourceService")?.GetValue(extraParams);
        _battleService = (IBattleService)type.GetProperty("_battleService")?.GetValue(extraParams);
        _windowManager = (WindowManager)type.GetProperty("_windowManager")?.GetValue(extraParams);

        if (gameSave.GetData(SaveConstants.TutorialOne, false))
        {
            ShowBattleButton(true);
            Destroy(gameObject);
            return;
        }

        _gameSave = gameSave;

        _windowManager = windowManager;
        _playerSpawner = playerSpawner;
        
        _fightButtonRect = _windowManager.GetWindow<WindowGame>().FightButton;
        _fightButtonScale = _fightButtonRect.localScale;

        _playerSpawner.AddListener(UpdatePlayerStata);
        
        _cursorStartPosition = _cursor.localPosition;

        _cursorParticleScale = _cursorParticle.transform.localScale;
        
        DisableButton().Forget();
    }

    private void Start()
    {
        tutorialJoystick.Construct(_windowManager);
        tutorialJoystick.OnMoving += CloseJoystick;
        _tileBuild = FindObjectsOfType<CustomBuilderTile>().Where(tile => tile.name == "CustomBuild").OrderBy(x => x.gameObject.activeSelf).First();
        _tileBuildComplete = FindObjectsOfType<TileBuild>().Where(tile => tile.name == "Tile_3_T").OrderBy(x => x.Cost).First();
        _tileBuildComplete.SetStateVisibleBuild(false);
        Faze = 1;
    }

    private void CloseJoystick()
    {
        tutorialJoystick.gameObject.SetActive(false);
    }

    private async UniTaskVoid DisableButton()
    {
        await UniTask.DelayFrame(5);
        ShowBattleButton(false);
    }

    private void InitializeLogs(CollectionTutorialResource[] resources, IGameResourceService resourceService)
    {
        GtapAnalytics.TutorialStepStart(1);
        foreach (var collectable in resources)
        {
            var bubble = Instantiate(bubbleCollectable, collectable.transform.position, Quaternion.identity);
            bubble.SetItem(collectable);
            collectable.Construct(resourceService);
            _collectingResource.Add(collectable);
        }
        ChangeLine();
    }

    private void ChangeLine()
    {
        if (_current == null)
        {
            if (_collectingResource.Count == 0)
            {
                _lineRenderer.SetStateLine(false);
                return;
            }
            var target = _collectingResource.GetRandomItem();
            _current = target.transform;
        }
    }

    private void Update()
    {
        if (_player != null && _current != null && _lineRenderer != null)
        {
            _lineRenderer.SetPosition(0, _player.position);
            _lineRenderer.SetPosition(1, _current.position);
        }
    }

    private void OnCollectedFazeOne(CollectingResourceObject resourceObject)
    {
        if (_collectingResource.Contains(resourceObject))
        {
            _collectingResource.Remove(resourceObject);
            if (_current == resourceObject.transform)
            {
                _current = null;
                ChangeLine();
            }
        }
        _countCollectedFazeOne++;
        if (_countCollectedFazeOne >= 3)
        {
            Faze = 2;
        }
    }

    private void OnCollectedFazeTwo(CollectingResourceObject resourceObject)
    {
        if (_collectingResource.Contains(resourceObject))
        {
            _collectingResource.Remove(resourceObject);
            if (_current == resourceObject.transform)
            {
                _current = null;
                ChangeLine();
            }
        }
        _countCollectedFazeTwo++;
        if (_countCollectedFazeTwo >= 3)
        {
            Faze = 4;
        }
    }
    
    private void OnCollectedFazeThree(CollectingResourceObject resource)
    {
        if (_collectingResource.Contains(resource))
        {
            _collectingResource.Remove(resource);
            if (_current == resource.transform)
            {
                _current = null;
                ChangeLine();
            }
        }
        _countCollectedFazeThree++;
        if (_countCollectedFazeThree >= 5)
        {
            Faze = 6;
        }
    }

    private EPlayerStates _currentPlayerState;
    private IJoystickService _joystickService;

    private void UpdatePlayerStata(EPlayerStates state, EntityPlayer player)
    {
        _currentPlayerState = state;
        switch (state)
        {
            case EPlayerStates.NotPlayer:
                break;
            case EPlayerStates.SpawnPlayer:
                _player = player.Hips;
                break;
            case EPlayerStates.PlayerInRaft:
                break;
            case EPlayerStates.PlayerInWater:
                break;
            case EPlayerStates.PlayerInBattle:
                break;
            case EPlayerStates.PlayerDead:
                break;
            case EPlayerStates.PlayerDeadInRaft:
                break;
            case EPlayerStates.PlayerDeadInWater:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void ShowCursor()
    {
        GtapAnalytics.TutorialStepStart(4);
        
        
        ShowBattleButton(true);

        PerformCursor();
    }

    private void PerformCursor()
    {
        _sequence.Kill();
        _fightButtonRect.DOKill();

        _cursor.gameObject.SetActive(true);
        _cursor.transform.SetParent(_fightButtonRect);
        _cursorParticle.transform.SetParent(_fightButtonRect);
        _fightButtonRect.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f).From(Vector3.one).SetEase(Ease.InQuad).SetLoops(int.MaxValue, LoopType.Yoyo);
        _sequence = DOTween.Sequence();
        _sequence.Append(_cursor.DOMove(_fightButtonRect.position + new Vector3(-50f, -10, 0), 1).From(_cursorStartPosition)
            .OnComplete(() =>
            {
                _cursorParticle.SetActive(true);
                _cursorParticle.transform.position = effectPosition.position;
            }));
        _cursor.localScale = Vector3.one;
        _cursor.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.5f).SetLoops(-1, LoopType.Yoyo);
        //_sequence.Append(_cursor.DOScale(0.95f, 1).SetLoops(int.MaxValue, LoopType.Yoyo)).Join(_cursorParticle.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.5f).From(Vector3.one).SetLoops(int.MaxValue, LoopType.Yoyo));
    }

    protected override void Complete()
    {
        GtapAnalytics.TutorialStepComplete();

        _gameSave.SetData(SaveConstants.TutorialOne, true);
        

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        tutorialJoystick.OnMoving -= CloseJoystick;
        
        _fightButtonRect.DOKill();
        if (_fightButtonRect)
        {
            _fightButtonRect.DOKill();
        }

        _sequence.Kill();
        if (_playerSpawner != null)
        {
            _playerSpawner.RemoveListener(UpdatePlayerStata);
        }
    }
}
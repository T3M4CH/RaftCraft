using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Scripts.BattleMode;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.InteractiveObjects.Interfaces;
using Game.Scripts.InteractiveObjects.UI;
using Game.Scripts.NPC.Fish;
using Game.Scripts.Quest.Interfaces;
using Game.Scripts.Raft.Components;
using Game.Scripts.Raft.SaveAndLoad;
using Game.Scripts.ResourceController.Interfaces;
using GTap.Analytics;
using GTapSoundManager.SoundManager;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Raft.BuildSystem
{
    public class TileBuild : MonoBehaviour, IInteraction, IReadyClient
    {
        public enum FazeBuild
        {
            One,
            Two
        }

        [System.Serializable]
        public class PartBuild
        {
            public FazeBuild Faze;
            public TileRaft Parent;
            public List<PartBuildTile> Parts;
        }

        [System.Serializable]
        public class PartBuildTile
        {
            public Transform TransformPart;
            public Vector3 StartPositionPart;
            public Vector3 StartRotationPart;
            public Vector3 StartScaleObject;

            [Button]
            private void Init()
            {
                StartPositionPart = TransformPart.localPosition;
                StartRotationPart = TransformPart.eulerAngles;
                StartScaleObject = TransformPart.localScale;
            }
        }

        public Action OnBuyTile;
        public Action OnStartBuilding;
        public event Action<BuildState, TileBuild> OnStateChanged = (x, y) => { };
        [SerializeField, FoldoutGroup("Constructor")] private string _nameTile;
        
        [SerializeField, FoldoutGroup("Constructor"), ReadOnly] private RowRaft _parentRow;
        [SerializeField, FoldoutGroup("Constructor")] private List<PartBuild> _partBuild = new List<PartBuild>();

        [SerializeField, FoldoutGroup("Constructor")] private TileBuild _parentBuild;

        [SerializeField, FoldoutGroup("Event")] private UnityEvent OnFullBuy;
        [SerializeField, FoldoutGroup("Event")] private UnityEvent OnClose;
        [SerializeField, FoldoutGroup("Event")] private UnityEvent OnVisibleBuild;
        
        [SerializeField, FoldoutGroup("View")] private BuildCostView _viewCost;
        [SerializeField, FoldoutGroup("View")] private TileUIController _uiController;
        [SerializeField, FoldoutGroup("View")] private Collider _coliderInteraction;
        [SerializeField, FoldoutGroup("View")] private TileConstructor _constructor;
        [SerializeField, FoldoutGroup("Feedback")] private Vector3 _offsetStartPoint;

        [SerializeField, FoldoutGroup("Feedback")] private float _durationOpen;

        [SerializeField, FoldoutGroup("Feedback")] private float _durationBuild;

        [SerializeField, FoldoutGroup("Feedback")] private float _delayOpen;

        [SerializeField, FoldoutGroup("Feedback")] private Ease _easeOpen;

        [SerializeField, FoldoutGroup("Feedback")] private Vector3 _targetScale;

        [SerializeField, FoldoutGroup("Default Data")] private DropItems _costBuild;

        [SerializeField, FoldoutGroup("Default Data")] private float _timeBuild;

        [SerializeField, FoldoutGroup("Default Data")] private BuildState _defaultState;

        private int _countResource;

        public int CountResource
        {
            get => _countResource;

            private set
            {
                _countResource = Mathf.Clamp(value, 0, int.MaxValue);
                _viewCost.SetCount(_countResource);
                _viewCost.SetProgress(1f - ((float)_countResource / _costBuild.Count));
                if (_countResource == 0 && State == BuildState.VisibleOnBuild)
                {
                    if (string.IsNullOrEmpty(_nameTile) == false)
                    {
                        GtapAnalytics.BuildTile(_nameTile);
                    }

                    OnStartBuilding?.Invoke();
                    State = BuildState.Building;
                    Timer = _timeBuild;
                }
            }
        }

        private float _timer;
        private SoundAsset _buildingSound;

        public float Timer
        {
            get => _timer;

            private set
            {
                _timer = Mathf.Clamp(value, 0f, float.MaxValue);
                _viewCost.SetProgressTime(_timer);
                _viewCost.SetProgressTimer(1f - (_timer / _timeBuild));
                if (_timer == 0 && State == BuildState.Building)
                {
                    
                    PlayerAnimationBuild();
                    OnBuyTile?.Invoke();
                }
            }
        }

        #region Public Field

        public InteractionType CurrentTypeInteraction => InteractionType.Build;

        private const float DefaultDelay = 0.1f;
        private const float DelayMultiplier = 0.95f;
        private float _currentDelay;
        public float DelayAction => _currentDelay;
        [field: SerializeField] public bool IsAbleEverywhere => false;
        public bool Interaction => State == BuildState.VisibleOnBuild;

        public bool HaveConstruct => State == BuildState.UnLock || State == BuildState.Building;

        #endregion

        #region Private Field

        private int _costAction;

        #endregion

        public BuildState GlobalState => _state;

        private BuildState _state;

        [ShowInInspector, ReadOnly]
        public BuildState State
        {
            get => _state;

            private set
            {
                _state = value;

                OnStateChanged.Invoke(_state, this);
                Debug.Log($"Set state: {_state}");
                switch (_state)
                {
                    case BuildState.Lock:
                        _isReady = true;
                        OnClose?.Invoke();
                        _viewCost.gameObject.SetActive(false);
                        _uiController.SetBigPanelState(false);
                        _coliderInteraction.enabled = false;
                        break;
                    case BuildState.VisibleOnBuild:
                        OnVisibleBuild?.Invoke();
                        _isReady = true;
                        _viewCost.gameObject.SetActive(true);
                        _viewCost.SetStatePanelPrice(true);
                        _viewCost.SetStatePanelTimer(false);
                        _uiController.SetBigPanelState(_constructor.IsShowBigPanel);
                        _coliderInteraction.enabled = true;
                        break;
                    case BuildState.Building:
                        _isReady = false;
                        _viewCost.SetStatePanelPrice(false);
                        _viewCost.SetStatePanelTimer(true);
                        StartCoroutine(WaitTimer());
                        break;
                    case BuildState.UnLock:
                        _isReady = true;
                        OnBuyTile?.Invoke();
                        OnFullBuy?.Invoke();
                        _uiController.SetBigPanelState(false);
                        _viewCost.gameObject.SetActive(false);
                        _coliderInteraction.enabled = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                OnChangeReady?.Invoke(this);
            }
        }

        private IResourceService _resourceService;
        private IBattleService _battleService;
        private IReadyService _readyService;
        
        public Vector3 PositionUI
        {
            get { return _viewCost.transform.position; }
        }

        [Button]
        private void InitRow()
        {
            if (_parentRow == null)
            {
                _parentRow = GetComponentInParent<RowRaft>();
            }
        }

        public void SetNameTile(string nameTile)
        {
            _nameTile = nameTile;
        }

        private void OnBuildParent()
        {
            if (CountResource == 0 && State != BuildState.Building && State != BuildState.UnLock)
            {
                State = BuildState.Building;
            }
        }

        private void OnDisable()
        {
            if (_parentBuild == null)
            {
                return;
            }

            _parentBuild.OnBuyTile -= OnBuildParent;
        }

        [Inject]
        public void Construct(IResourceService resourceService, IBattleService battleService, IReadyService readyService)
        {
            _readyService = readyService;
            _battleService = battleService;
            _resourceService = resourceService;
        }

        private void Start()
        {
            _buildingSound = Resources.Load<SoundAsset>("SoundAssets/Xylophone");
            _readyService.AddClient(this);
            _currentDelay = DefaultDelay;
            _battleService.OnChangeState += OnBattleChange;
        }

        private IEnumerator WaitTimer()
        {
            while (Timer > 0)
            {
                yield return null;
                Timer -= Time.smoothDeltaTime;
            }
        }

        public void UnLockBuild()
        {
            if (State == BuildState.Lock)
            {
                State = BuildState.VisibleOnBuild;
            }
        }

        private void PlayerAnimationBuild()
        {
            State = BuildState.UnLock;
            BuildToMove(FazeBuild.One, _parentBuild != null && _parentBuild.transform.localPosition.x < transform.localPosition.x);
            BuildToMove(FazeBuild.Two, _parentBuild != null && _parentBuild.transform.localPosition.x < transform.localPosition.x);
        }

        public void SetStateVisibleBuild(bool state)
        {
            State = state ? BuildState.VisibleOnBuild : BuildState.Lock;
            _viewCost.gameObject.SetActive(state);
            _coliderInteraction.enabled = state;
        }

        public void UpdatePositionBuild(Vector3 position)
        {
            _uiController.SetPosition(position);
        }

        public void UpdateStateBigPanel(bool state)
        {
            _uiController.SetBigPanelState(state);
        }

        public void SetLoadedData(DataTile data)
        {
            Timer = data.TimeBuild;
            State = data.StateBuild;
            CountResource = data.CountResource;
            SetFastState(State == BuildState.UnLock || Timer == 0, FazeBuild.One);
            if (_parentBuild == null)
            {
                SetStateFazeTow(State == BuildState.UnLock || State == BuildState.VisibleOnBuild);
            }
            else
            {
                SetStateFazeTow(State == BuildState.UnLock);
            }
            if (CountResource == 0 && State == BuildState.Building)
            {
                SetStateFazeTow(true);
            }
            if (_parentBuild == null)
            {
                return;
            }

            _parentBuild.OnBuyTile += OnBuildParent;
        }

        public void SetStateFazeTow(bool state)
        {
            SetFastState(state, FazeBuild.Two);
        }

        public void LoadDefault()
        {
            CountResource = _costBuild.Count;
            State = _defaultState;
            Timer = _timeBuild;
            SetFastState(State == BuildState.UnLock, FazeBuild.One);
            SetFastState(false, FazeBuild.Two);
            if (_parentBuild == null)
            {
                return;
            }

            _parentBuild.OnBuyTile += OnBuildParent;
        }

        public void BuildFazeTow()
        {
            BuildToMove(FazeBuild.One);
            BuildToMove(FazeBuild.Two);
        }

        #region Build Methods

        public void Action()
        {
            if (CountResource > 0)
            {
                if (IsAbleEverywhere)
                {
                    if (_resourceService.TryRemoveLocal(_costBuild.ResourceType, 1))
                    {
                        _currentDelay *= DelayMultiplier;
                        CountResource--;
                        _viewCost.PlayEffect();
                    }

                    return;
                }

                if (_resourceService.TryRemove(_costBuild.ResourceType, 1))
                {
                    _currentDelay *= DelayMultiplier;
                    CountResource--;
                    _viewCost.PlayEffect();
                }
            }
        }

        public void SetState(BuildState buildState)
        {
            State = buildState;
        }

        public void EnterInteraction()
        {
            _costAction = 1;
            if (!IsAbleEverywhere && _resourceService.HaveCount(_costBuild.ResourceType, _costAction) == false)
            {
                _viewCost.PlayEffectNotItems();
            }
        }

        public void ExitInteraction()
        {
            _currentDelay = DefaultDelay;
        }

        #endregion

        private bool _buildingOne = false;
        private bool _buildingTwo = false;

        [Button]
        private void BuildToMove(FazeBuild faze, bool left = true)
        {
            switch (faze)
            {
                case FazeBuild.One:
                    if (_buildingOne)
                    {
                        return;
                    }

                    _buildingOne = true;
                    break;
                case FazeBuild.Two:
                    if (_buildingTwo)
                    {
                        return;
                    }

                    _buildingTwo = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faze), faze, null);
            }

            var delay = left ? 0f : _delayOpen * CountFragments(faze);
            foreach (var tile in _partBuild)
            {
                tile.Parent.EnableTile();
                if (tile.Faze != faze)
                {
                    continue;
                }

                var lenght = tile.Parts.Count;
                for (var i = 0; i < lenght; i++)
                {
                    var part = tile.Parts[i];
                    if (part.TransformPart.gameObject.activeSelf == false)
                    {
                        continue;
                    }

                    if (part.TransformPart.transform.localScale != Vector3.zero)
                    {
                        continue;
                    }

                    var sequence = DOTween.Sequence();
                    part.TransformPart.DOKill();
                    part.StartPositionPart = part.TransformPart.localPosition;
                    part.TransformPart.localPosition = part.StartPositionPart + _offsetStartPoint;
                    var startScale = part.StartScaleObject;
                    part.TransformPart.localScale = Vector3.zero;
                    sequence.AppendInterval(delay);
                    sequence.Append(
                        part.TransformPart.DOScale(_targetScale * startScale.x, _durationOpen).SetEase(_easeOpen)
                            .OnComplete(() =>
                            {
                                _buildingSound.Play(Mathf.Lerp(0.5f,1, i / (float)lenght));
                                part.TransformPart.DOScale(startScale, _durationBuild).SetEase(_easeOpen);
                            }));
                    sequence.Join(part.TransformPart.DOLocalMove(part.StartPositionPart, _durationOpen).SetEase(_easeOpen));
                    sequence.Join(part.TransformPart.DOLocalRotate(part.StartRotationPart, _durationOpen).SetEase(_easeOpen));
                    sequence.SetLink(part.TransformPart.gameObject);
                    delay += left ? _delayOpen : -_delayOpen;
                }
            }
        }

        private void SetFastState(bool state, FazeBuild fazeBuild)
        {
            foreach (var tile in _partBuild)
            {
                if (tile.Faze != fazeBuild)
                {
                    continue;
                }

                if (state)
                {
                    tile.Parent.EnableTile();
                }
                else
                {
                    tile.Parent.DisableTile();
                }

                foreach (var part in tile.Parts)
                {
                    part.TransformPart.DOKill();
                    part.TransformPart.localScale = state ? part.StartScaleObject : Vector3.zero;
                }
            }
        }

        private int CountFragments(FazeBuild partBuilds)
        {
            var result = 1;
            foreach (var floor in _partBuild)
            {
                if (floor.Faze == partBuilds)
                {
                    result += floor.Parts.Count;
                }
            }

            return result;
        }

        [Button]
        private void Close(FazeBuild fazeBuild)
        {
            foreach (var fragment in _partBuild)
            {
                if (fragment.Faze != fazeBuild)
                {
                    continue;
                }

                foreach (var part in fragment.Parts)
                {
                    part.TransformPart.DOKill();
                    part.TransformPart.localScale = Vector3.zero;
                }
            }
        }

        public void AddTile(TileRaft tileRaft)
        {
            if (TryAddTile(tileRaft, out var build))
            {
                _partBuild.Add(build);
                if (build.Parts == null || build.Parts.Count == 0)
                {
                    build.Parts.Add(new PartBuildTile()
                    {
                        StartRotationPart = tileRaft.transform.eulerAngles,
                        StartPositionPart = tileRaft.transform.localPosition,
                        StartScaleObject = tileRaft.transform.localScale,
                        TransformPart = tileRaft.transform
                    });
                }

                var list = _partBuild.OrderBy(p => p.Parent.transform.localPosition.y);
                _partBuild = list.ToList();
            }
            else
            {
            }
        }

        private bool TryAddTile(TileRaft input, out PartBuild part)
        {
            foreach (var build in _partBuild)
            {
                if (build.Parent == input)
                {
                    part = null;
                    return false;
                }
            }

            var result = new PartBuild()
            {
                Faze = FazeBuild.Two,
                Parent = input,
                Parts = ConvertToData(input.Fragments)
            };

            part = result;
            return true;
        }

        private bool TryGet(TileRaft input, out PartBuild output)
        {
            foreach (var part in _partBuild)
            {
                if (part.Parent == input)
                {
                    output = part;
                    return true;
                }
            }

            output = null;
            return false;
        }

        public void RemoveTile(TileRaft tileRaft)
        {
            if (TryGet(tileRaft, out var build))
            {
                _partBuild.Remove(build);
            }
        }

        [Button]
        private List<PartBuildTile> ConvertToData(List<TileRaft.FragmentData> data)
        {
            var result = new List<PartBuildTile>();
            foreach (var d in data)
            {
                result.Add(new PartBuildTile()
                {
                    StartRotationPart = d.Transform.transform.eulerAngles,
                    StartPositionPart = d.Transform.transform.localPosition,
                    StartScaleObject = d.Transform.transform.localScale,
                    TransformPart = d.Transform.transform
                });
            }

            return result;
        }

        private void OnBattleChange(BattleState battleState)
        {
            if (State == BuildState.VisibleOnBuild)
            {
                _viewCost.gameObject.SetActive(battleState == BattleState.Idle);
            }
        }

        private void OnDestroy()
        {
            _readyService.RemoveClient(this);
            foreach (var part in _partBuild)
            {
                foreach (var partBuild in part.Parts)
                {
                    if (partBuild.TransformPart != null)
                    {
                        partBuild.TransformPart.DOKill();
                    }
                }
            }

            _battleService.OnChangeState -= OnBattleChange;
        }

        public int Cost => _costBuild.Count;
        public Transform UITransform => _viewCost.transform;
        public event Action<IReadyClient> OnChangeReady;
        private bool _isReady;
        public bool IsReady => _isReady;
        public float Delay => 0f;
    }
}
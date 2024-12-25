using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.InteractiveObjects.Interfaces;
using Game.Scripts.InteractiveObjects.UI;
using Game.Scripts.Raft.BuildSystem;
using Game.Scripts.Raft.Components;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using GTap.Analytics;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Raft.CustomBuilder
{
    public class CustomBuilderTile : MonoBehaviour, IInteraction
    {
        public enum CustomBuildState
        {
            None,
            BuildLeft,
            BuildingLeft,
            UnLockLeft,
            BuildRight,
            BuildingRight,
            UnLockRight,
            UnLock
        }
        
        [System.Serializable]
        public class ColliderData
        {
            public Vector3 Size;
            public Vector3 Center;
        }

        public event Action OnBuildLeft; 
        public event Action OnBuildRight;
        public event Action OnStartBuilding;
        
        [SerializeField, ReadOnly]private string _guid;

        [SerializeField, FoldoutGroup("Build")] private List<TileBuild.PartBuild> _partLeft;
        [SerializeField, FoldoutGroup("Build")] private List<TileBuild.PartBuild> _partRight;
        [SerializeField, FoldoutGroup("Build")] private ColliderData _dataLock;
        [SerializeField, FoldoutGroup("Build")] private ColliderData _dataLeft;
        [SerializeField, FoldoutGroup("Build")] private ColliderData _dataRight;
        [SerializeField, FoldoutGroup("Build")] private BoxCollider _collider;
        
        [SerializeField, FoldoutGroup("Controller")] private bool _haveEnabled = false;
        [SerializeField, FoldoutGroup("Settings")] private BuildCostView _costView;

        [SerializeField, FoldoutGroup("Cost")] private EResourceType _resourceType;
        [SerializeField, FoldoutGroup("Cost")] private int _priceLeft;
        [SerializeField, FoldoutGroup("Cost")] private int _priceRight;
        [SerializeField, FoldoutGroup("Cost")] private float _timeBuild;
        
        [SerializeField, FoldoutGroup("Build Settings")] private Vector3 _positionBuildLeft;
        [SerializeField, FoldoutGroup("Build Settings")] private Vector3 _positionBuildRight;
        [SerializeField, FoldoutGroup("Feedback")] private Vector3 _offsetStartPoint;
        [SerializeField, FoldoutGroup("Feedback")] private Vector3 _targetScale;
        [SerializeField, FoldoutGroup("Feedback")] private float _durationOpen;
        [SerializeField, FoldoutGroup("Feedback")] private float _durationBuild;
        [SerializeField, FoldoutGroup("Feedback")] private Ease _easeOpen;
        

        private CustomBuildState _stateBuild;

        private CustomBuildState StateBuild
        {
            get => _stateBuild;

            set
            {
                _stateBuild = value;
                _costView.gameObject.SetActive(true);
                switch (_stateBuild)
                {
                    case CustomBuildState.None:
                        CloseParts();
                        StateBuild = CustomBuildState.BuildLeft;
                        CountResource = _priceLeft;
                        _collider.center = _dataLock.Center;
                        _collider.size = _dataLock.Size;
                        break;
                    case CustomBuildState.BuildLeft:
                        transform.localPosition = _positionBuildLeft;
                        _costView.SetStatePanelPrice(true);
                        _costView.SetStatePanelTimer(false);
                        break;
                    case CustomBuildState.BuildingLeft:
                        OnStartBuilding?.Invoke();
                        _costView.SetStatePanelPrice(false);
                        _costView.SetStatePanelTimer(true);
                        StopAllCoroutines();
                        StartCoroutine(WaitBuild(true));
                        break;
                    case CustomBuildState.UnLockLeft:
                        OnBuildLeft?.Invoke();
                        CountResource = _priceRight;
                        break;
                    case CustomBuildState.BuildRight:
                        _costView.SetStatePanelPrice(true);
                        _costView.SetStatePanelTimer(false);
                        transform.localPosition = _positionBuildRight;
                        break;
                    case CustomBuildState.BuildingRight:
                        OnStartBuilding?.Invoke();
                        _costView.SetStatePanelPrice(false);
                        _costView.SetStatePanelTimer(true);
                        StopAllCoroutines();
                        StartCoroutine(WaitBuild(false));
                        break;
                    case CustomBuildState.UnLockRight:
                        OnBuildRight?.Invoke();
                        StateBuild = CustomBuildState.UnLock;
                        break;
                    case CustomBuildState.UnLock:
                        _gameSave.SetData($"CustomBuild_{_guid}", CustomBuildState.UnLock);
                        _costView.gameObject.SetActive(false);
                        BuildLeft(true);
                        BuildRight(true);
                        _collider.center = _dataRight.Center;
                        _collider.size = _dataRight.Size;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private int _countResource;

        private int CountResource
        {
            get => _countResource;
            set
            {
                _countResource = Mathf.Clamp(value, 0, int.MaxValue);
                _costView.SetCount(_countResource);
                _costView.SetProgress(Progress);
                if (_countResource <= 0)
                {
                    if (StateBuild == CustomBuildState.BuildLeft)
                    {
                        GtapAnalytics.BuildTile($"Tutorial_01");
                        StateBuild = CustomBuildState.BuildingLeft;
                    }

                    if (StateBuild == CustomBuildState.BuildRight)
                    {
                        GtapAnalytics.BuildTile($"Tutorial_02");
                        StateBuild = CustomBuildState.BuildingRight;
                    }
                }
            }
        }

        private float _timeProgress;

        private float TimeProgress
        {
            get => _timeProgress;

            set
            {
                _timeProgress = Mathf.Clamp(value, 0f, float.MaxValue);
                _costView.SetProgressTime(_timeProgress);
                _costView.SetProgressTimer(1f - (_timeProgress / _timeBuild));
            }
        }

        private float Progress
        {
            get
            {
                switch (StateBuild)
                {
                    case CustomBuildState.None:
                        return 0f;
                    case CustomBuildState.BuildLeft:
                        return 1f - ((float)CountResource / _priceLeft);
                    case CustomBuildState.BuildingLeft:
                        return 0f;
                    case CustomBuildState.UnLockLeft:
                        return 0f;
                    case CustomBuildState.BuildRight:
                        return 1f - ((float)CountResource / _priceRight);
                    case CustomBuildState.BuildingRight:
                        return 0f;
                    case CustomBuildState.UnLockRight:
                        return 0f;
                    case CustomBuildState.UnLock:
                        return 0f;
                    default:
                        return 0f;
                }
            }
        }
        
        private IResourceService _resourceService;
        private GameSave _gameSave;
        private bool _scale = false;

        public int Cost => 0;

        public Vector3 PositionUI => _costView.transform.position;
        public Transform TransformUI => _costView.transform;

        private void OnEnable()
        {
            if (_haveEnabled == false)
            {
                gameObject.SetActive(false);
            }
        }
        

        [Inject]
        private void Construct(IResourceService resourceService, GameSave gameSave)
        {
            _resourceService = resourceService;
            _gameSave = gameSave;
        }

        private void Start()
        {
            if (_gameSave.GetData(SaveConstants.TutorialOne, false))
            {
                StateBuild = CustomBuildState.UnLock;
            }
            StartCoroutine(WaitLoad());
        }

        private IEnumerator WaitLoad()
        {
            yield return null;
            if (_gameSave.GetData(SaveConstants.TutorialOne, false) == false)
            {
                _collider.center = _dataLock.Center;
                _collider.size = _dataLock.Size;
                CloseParts();
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_guid))
            {
                _guid = Guid.NewGuid().ToString();
            }
        }

        public void StartLogic()
        {
            if (_gameSave.GetData(SaveConstants.TutorialOne, false) == false)
            {
                StateBuild = CustomBuildState.None;
            }
        }


        public void StartBuildRight()
        {
            StateBuild = CustomBuildState.BuildRight;
        }

        private void CloseParts()
        {
            foreach (var parts in _partLeft)
            {
                foreach (var part in parts.Parts)
                {
                    part.TransformPart.localScale = Vector3.zero;
                }
            }
            
            foreach (var parts in _partRight)
            {
                foreach (var part in parts.Parts)
                {
                    part.TransformPart.localScale = Vector3.zero;
                }
            }
        }

        [Button]
        private void BuildLeft(bool fast = false)
        {
            var delay = 0f;
            foreach (var part in _partLeft)
            {
                var lenght = part.Parts.Count;
                for (var i = lenght; i > 0; i--)
                {
                    var parts = part.Parts[i - 1];
                    var sequence = DOTween.Sequence();
                    parts.TransformPart.DOKill();
                    if (fast)
                    {
                        parts.TransformPart.localScale = parts.StartScaleObject;
                        parts.TransformPart.rotation = Quaternion.Euler(parts.StartRotationPart);
                        parts.TransformPart.localPosition = parts.StartPositionPart;
                    }
                    else
                    {
                        parts.StartPositionPart = parts.TransformPart.localPosition;
                        parts.TransformPart.localPosition = parts.StartPositionPart + _offsetStartPoint;
                        var startScale = parts.StartScaleObject;
                        parts.TransformPart.localScale = Vector3.zero;
                        sequence.AppendInterval(delay);
                        sequence.Append(
                            parts.TransformPart.DOScale(_targetScale * startScale.x, _durationOpen).SetEase(_easeOpen)
                                .OnComplete(() =>
                                {
                                    parts.TransformPart.DOScale(startScale, _durationBuild).SetEase(_easeOpen);
                                }));
                        sequence.Join(parts.TransformPart.DOLocalMove(parts.StartPositionPart, _durationOpen).SetEase(_easeOpen));
                        sequence.Join(parts.TransformPart.DOLocalRotate(parts.StartRotationPart, _durationOpen).SetEase(_easeOpen));
                        sequence.SetLink(parts.TransformPart.gameObject);
                        delay += 0.1f;   
                    }
                }
            }
        }

        [Button]
        private void BuildRight(bool fast = false)
        {
            var delay = 0f;
            foreach (var part in _partRight)
            {
                var lenght = part.Parts.Count;
                for (var i = lenght; i > 0; i--)
                {
                    var parts = part.Parts[i - 1];
                    var sequence = DOTween.Sequence();
                    parts.TransformPart.DOKill();
                    if (fast)
                    {
                        parts.TransformPart.localScale = parts.StartScaleObject;
                        parts.TransformPart.rotation = Quaternion.Euler(parts.StartRotationPart);
                        parts.TransformPart.localPosition = parts.StartPositionPart;
                    }
                    else
                    {
                        parts.StartPositionPart = parts.TransformPart.localPosition;
                        parts.TransformPart.localPosition = parts.StartPositionPart + _offsetStartPoint;
                        var startScale = parts.StartScaleObject;
                        parts.TransformPart.localScale = Vector3.zero;
                        sequence.AppendInterval(delay);
                        sequence.Append(
                            parts.TransformPart.DOScale(_targetScale * startScale.x, _durationBuild).SetEase(_easeOpen)
                                .OnComplete(() =>
                                {
                                    parts.TransformPart.DOScale(startScale, _durationBuild).SetEase(_easeOpen);
                                }));
                        sequence.Join(parts.TransformPart.DOLocalMove(parts.StartPositionPart, _durationOpen).SetEase(_easeOpen));
                        sequence.Join(parts.TransformPart.DOLocalRotate(parts.StartRotationPart, _durationOpen).SetEase(_easeOpen));
                        sequence.SetLink(parts.TransformPart.gameObject);
                        delay += 0.1f;   
                    }
                }
            }
        }

        private void Update()
        {
            if (_scale == false && StateBuild == CustomBuildState.None)
            {
                Debug.Log($"Reset Scale");
                _scale = true;
                _costView.SetStatePanelPrice(false);
                _costView.SetStatePanelTimer(false);
            }
        }

        [Button]
        private void Test(bool left)
        {
            StartCoroutine(WaitBuild(left));
        }

        private IEnumerator WaitBuild(bool left)
        {
            TimeProgress = _timeBuild;
            yield return StartCoroutine(WaitTimer());
            if (left)
            {
                _collider.center = _dataLeft.Center;
                _collider.size = _dataLeft.Size;
                BuildLeft();
            }
            else
            {
                _collider.center = _dataRight.Center;
                _collider.size = _dataRight.Size;
                BuildRight();
            }
            _costView.SetStatePanelTimer(false);
            _costView.SetStatePanelPrice(false);
            yield return new WaitForSeconds(1f);
            StateBuild = left ? CustomBuildState.UnLockLeft : CustomBuildState.UnLockRight;
        }

        private IEnumerator WaitTimer()
        {
            while (TimeProgress > 0f)
            {
                TimeProgress -= Time.smoothDeltaTime;
                yield return null;
            }
        }

        public bool IsAbleEverywhere => false;
        public bool Interaction => StateBuild is CustomBuildState.BuildLeft or CustomBuildState.BuildRight;
        public InteractionType CurrentTypeInteraction => InteractionType.Build;
        public float DelayAction => 0.1f;
        public void Action()
        {
            if (_resourceService.TryRemove(_resourceType, 1))
            {
                CountResource--;
                _costView.PlayEffect();
            }
        }

        public void EnterInteraction()
        {
            if (_resourceService.HaveCount(_resourceType, 1) == false)
            {
                _costView.PlayEffectNotItems();
            }
        }

        public void ExitInteraction()
        {
        }
    }
}

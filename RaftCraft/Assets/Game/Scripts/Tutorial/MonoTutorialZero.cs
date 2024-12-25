using System;
using System.Collections;
using DG.Tweening;
using Game.Scripts.CameraSystem.Cameras;
using Game.Scripts.CameraSystem.Interfaces;
using Game.Scripts.Days;
using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.TransitionEffect.Interfaces;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowManager.Windows;
using GTap.Analytics;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MonoTutorialZero : MonoBehaviour
{
    [SerializeField] private Transform _raft;
    [SerializeField] private Image _background;
    [SerializeField] private RectTransform _cursor;
    [SerializeField] private RectTransform _sailButtonRect;
    [SerializeField] private GameObject[] _disabledObjects;
    [SerializeField] private GameObject cursorParticle;
    [SerializeField] private Transform effectPosition;

    private bool _isTutorialComplete;
    private Sequence _sequence;
    private Sequence _sailButtonSequence;
    private Button _sailButton;
    private Vector3 _cursorStartPosition;
    private ICameraService _cameraService;
    private IPlayerService _playerSpawner;
    private IJoystickService _joystickService;
    private BiomesSceneManager _biomesSceneManager;
    private IDayService _dayController;
    private CanvasGroup _playerUi;
    private bool _moveToOcean;
    private Vector3 _endPosition;
    private EntityPlayer _player;

    [Inject]
    private void Construct
    (
        ICameraService cameraService,
        IPlayerService playerSpawner,
        BiomesSceneManager biomesSceneManager,
        WindowManager windowManager,
        IDayService dayController
    )
    {
        _dayController = dayController;
        _biomesSceneManager = biomesSceneManager;
        _joystickService = windowManager.GetWindow<WindowGame>().Joystick;
        _cameraService = cameraService;
        _playerSpawner = playerSpawner;
        _moveToOcean = false;
        _playerSpawner.AddListener(CreatePlayer);
    }

    private void CreatePlayer(EPlayerStates state, EntityPlayer player)
    {
        if (state == EPlayerStates.SpawnPlayer)
        {
            _player = player;
            _playerUi = player.transform.GetComponentInChildren<CanvasGroup>();
            _playerUi.alpha = 0;
        }

        GtapAnalytics.TutorialStepStart(0);
    }

    private void Start()
    {
        _cursorStartPosition = _cursor.position;

        _sailButton = _sailButtonRect.GetComponent<Button>();

        if (_sailButton == null)
        {
            throw new Exception("Sail button is Null");
        }

        _sailButton.onClick.AddListener(() =>
        {
            GtapAnalytics.TutorialStepComplete();
            
            StopButtonAnimation();
            PerformTransition();

            _isTutorialComplete = true;

            _sailButton.onClick.RemoveAllListeners();
        });
    }

    public void ChangeCamera()
    {
        _cameraService.ChangeActiveCamera<PlayerVirtualCamera>();
    }

    public void PerformButtonAnimation()
    {
        if (_isTutorialComplete) return;

        _sequence.Kill();
        _sailButtonSequence.Kill();

        _sequence = DOTween.Sequence();
        _sailButtonSequence = DOTween.Sequence();

        cursorParticle.SetActive(false);
        _sailButtonRect.gameObject.SetActive(true);

        _cursor.position = _cursorStartPosition;
        _cursor.localScale = Vector3.one;

        _cursor.gameObject.SetActive(true);

        _sailButtonSequence.Append(_sailButtonRect.DOScale(1.1f, 0.5f).SetEase(Ease.InQuad).SetLoops(int.MaxValue, LoopType.Yoyo));
        _sequence.Append(_cursor.DOLocalMove(_sailButtonRect.localPosition + new Vector3(55, -25, 0), 1.25f)
            .OnComplete(() =>
            {
                cursorParticle.SetActive(true);
                cursorParticle.transform.position = effectPosition.position;
            }));
        _sequence.Append(_cursor.DOScale(0.95f, 1).SetLoops(int.MaxValue, LoopType.Yoyo)).Join(cursorParticle.transform.DOScale(0.5f, 0.5f).SetLoops(int.MaxValue, LoopType.Yoyo));
    }

    public void StopButtonAnimation()
    {
        if (_isTutorialComplete) return;

        _sequence.Kill();
        _sailButtonSequence.Kill();

        cursorParticle.SetActive(false);
        _cursor.gameObject.SetActive(false);

        _sailButtonRect.gameObject.SetActive(false);
    }

    private void PerformTransition()
    {
        if (_player == null)
        {
            return;
        }

        for (var i = 0; i < _disabledObjects.Length; i++)
        {
            _disabledObjects[i].SetActive(false);
        }

        _biomesSceneManager.SaveNextLevel();


        _player.transform.SetParent(_raft, true);
        _player.Rb.isKinematic = true;
        _endPosition = _raft.transform.position + new Vector3(70f, 0f, 0f);
        _moveToOcean = true;
        _background.DOColor(Color.black, 3).OnComplete(() => { _biomesSceneManager.CompleteLevel(); });

        _joystickService.HideGUI();
    }

    private void PerformDayChange()
    {
    }

    private void FixedUpdate()
    {
        if (_moveToOcean == false)
        {
            return;
        }

        _raft.position = Vector3.Lerp(_raft.position, _endPosition, Time.fixedDeltaTime * 0.1f);
    }

    private void OnDestroy()
    {
        if (_isTutorialComplete)
        {
            _dayController.CompleteDay();
        }

        _sequence.Kill();
        _background.DOKill();
        _biomesSceneManager.OnSceneChanged -= PerformDayChange;
        if (_playerSpawner != null)
        {
            _playerSpawner.RemoveListener(CreatePlayer);
        }

        _sailButtonSequence.Kill();
    }
}
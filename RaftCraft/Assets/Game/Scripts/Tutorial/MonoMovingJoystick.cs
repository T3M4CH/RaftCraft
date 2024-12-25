using System;
using Game.Scripts.Joystick.Interfaces;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using System.Linq;
using DG.Tweening;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowManager.Windows;
using UnityEngine;

public class MonoMovingJoystick : MonoBehaviour
{
    public event Action OnMoving = () => { };
    
    [SerializeField] private RectTransform _handler;
    [SerializeField] private RectTransform[] _positions;
    
    private Sequence _sequence;
    private IJoystickService _joystickService;

    [Inject]
    public void Construct(WindowManager windowManager)
    {
        _joystickService = windowManager.GetWindow<WindowGame>().Joystick;
    }

    private void Update()
    {
        if (_joystickService.IsDragging)
        {
            OnMoving.Invoke();
            gameObject.SetActive(false);
        }
    }

    private async void OnEnable()
    {
        await UniTask.Yield(PlayerLoopTiming.LastUpdate);
        
        _sequence = DOTween.Sequence();

        var path = _positions.Select(position => position.localPosition).ToArray();
        
        _handler.localPosition = _positions[0].localPosition;
        _sequence.Append(_handler.DOLocalPath(path, 2.4f, PathType.CatmullRom));
        _sequence.SetLoops(int.MaxValue, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        _sequence.Kill();
    }

    private void OnDestroy()
    {
        _sequence.Kill();
    }
}

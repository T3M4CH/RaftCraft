using System;
using Game.Scripts.Joystick;
using Game.Scripts.Joystick.Extras;
using Game.Scripts.Joystick.Interfaces;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MonoJoystick : MonoBehaviour, IJoystickService
{
    public enum TypeJoystick
    {
        Default,
        Water
    }

    public event Action<DirectionJoystick> OnChangeDirection; 
    
    [SerializeField, Min(0)] private int _limitOffsetStickImage;
    [SerializeField, Min(0)] private float _speedStickImage;

    private bool _isLock;
    private PlayerInput _playerInput;
    [SerializeField] private Image _circleImage;
    [SerializeField] private Image _stickImage;

    [SerializeField] private Sprite _circleSpriteDefault;
    [SerializeField] private Sprite _stickSpriteDefault;

    [SerializeField] private Sprite _circleSpriteWater;
    [SerializeField] private Sprite _stickSpriteWater;

    [SerializeField] private Sprite _circleSpriteDefaultSelect;
    [SerializeField] private Sprite _circleSpriteWaterSelect;

    
    private RectTransform _rectCircle;
    private RectTransform _rectStick;
    private Vector3 _direction;

    private bool _isTouch;

    public bool IsTouch
    {
        get => _isTouch;

        private set
        {
            _isTouch = value;
            switch (CurrentType)
            {
                case TypeJoystick.Default:
                    _circleImage.sprite = _isTouch ? _circleSpriteWaterSelect : _circleSpriteWater;;
                    break;
                case TypeJoystick.Water:
                    _circleImage.sprite = _isTouch ? _circleSpriteWaterSelect : _circleSpriteWater;;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void HideGUI(bool isPersistent = true)
    {
        if (isPersistent)
        {
            _isLock = true;
            _direction = Vector2.zero;
        }
        
        _circleImage.enabled = false;
        _stickImage.enabled = false;
    }

    private TypeJoystick _currentType;

    private TypeJoystick CurrentType
    {
        get => _currentType;

        set
        {
            _currentType = value;
            switch (_currentType)
            {
                case TypeJoystick.Default:
                    _circleImage.sprite = _circleSpriteWater;
                    _stickImage.sprite = _stickSpriteWater;
                    break;
                case TypeJoystick.Water:
                    _circleImage.sprite = _circleSpriteWater;
                    _stickImage.sprite = _stickSpriteWater;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    

    public void SetStateDefault()
    {
        CurrentType = TypeJoystick.Default;
    }
    
    public void SetStateWater()
    {
        CurrentType = TypeJoystick.Water;
    }

    public void ReturnGUI(bool isPersistent = true)
    {
        if (isPersistent) _isLock = false;
        if (_isLock) return;
        
        _circleImage.enabled = true;
        _stickImage.enabled = true;
    }

    public bool IsDragging => IsTouch;
    public Vector3 Direction => new Vector3(_direction.x, 0f, _direction.y) / _limitOffsetStickImage;

    public event Action OnTouchDownEvent;
    public event Action OnTouchReleasedEvent;

    private float _valueUpdateDirection = 0.8f;
    
    private DirectionJoystick _directionJoystick;

    private DirectionJoystick TargetDirection
    {
        get => _directionJoystick;
        set
        {
            if (value == DirectionJoystick.Default)
            {
                _directionJoystick = value;
                return;
            }
            if (value != _directionJoystick)
            {
                _directionJoystick = value;
                OnChangeDirection?.Invoke(_directionJoystick);
            }
        }
    }
    
    public void Setup(InputSingleton playerInput)
    {
        _rectCircle = _circleImage.rectTransform;
        _rectStick = _stickImage.rectTransform;
        _playerInput = playerInput.Instance;

        Reset();
    }

    private void StartInput()
    {
        IsTouch = true;
        
        _rectCircle.position = _playerInput.Player.TouchPosition.ReadValue<Vector2>();
        _rectStick.position = _playerInput.Player.TouchPosition.ReadValue<Vector2>();
        _circleImage.enabled = true;
        _stickImage.enabled = true;
        
        OnTouchDownEvent?.Invoke();
    }

    private void PerformedInput()
    {
        var touch = _playerInput.Player.TouchPosition.ReadValue<Vector2>();
        var touchDirection = Vector3.ClampMagnitude((Vector3)touch - _rectCircle.position, _limitOffsetStickImage);
        _rectStick.anchoredPosition = Vector2.Lerp(_rectStick.anchoredPosition, touchDirection, Time.deltaTime * _speedStickImage);
        _direction = Vector2.Lerp(_direction, touchDirection, Time.deltaTime * _speedStickImage);
       
        TargetDirection = Direction.z >= _valueUpdateDirection ? DirectionJoystick.Up : DirectionJoystick.Default;

        TargetDirection = Direction.z <= -_valueUpdateDirection ? DirectionJoystick.Down : DirectionJoystick.Default;

        TargetDirection = Direction.x >= _valueUpdateDirection ? DirectionJoystick.Right : DirectionJoystick.Default;

        TargetDirection = Direction.x <= -_valueUpdateDirection ? DirectionJoystick.Left : DirectionJoystick.Default;
    }
    
    private void EndInput()
    {
        Reset();
        
        OnTouchReleasedEvent?.Invoke();
    }

    private void Update()
    {
        if(_isLock) return;
        
        if (_playerInput == null)
        {
            return;
        }
        
        if (_playerInput.Player.Touch.WasPressedThisFrame())
        {
            if(EventSystem.current.IsPointerOverGameObject(-1)) return;
            StartInput();
        }
        
        if (_playerInput.Player.Touch.WasReleasedThisFrame())
        {
            EndInput();
        }

        if (IsTouch)
        {
            PerformedInput();
        }    
    }

    private void Reset()
    {
        IsTouch = false;
        
        _direction = Vector2.zero;
        
        if (!_circleImage || !_stickImage) return;
        
        _circleImage.enabled = false;
        _stickImage.enabled = false;
    }
    
    private void OnDisable()
    {
        Reset();
    }
}
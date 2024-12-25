using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.Player;
using UnityEngine;

public class AscentController : MonoBehaviour
{
    [SerializeField] private EntityPlayer entityPlayer;

    private float _currentTime;
    private float _mass;
    private Rigidbody _rigidbody;
    private Transform _playerTransform;
    private IJoystickService _joystickService;

    private const float Delay = 2f;

    private void Update()
    {
        if(_playerTransform.position.y >= -1) return;
        
        if (!_joystickService.IsDragging)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            _currentTime = 0;
            return;
        }

        if (_currentTime > Delay)
        {
            _rigidbody.AddForce(Vector3.down * (-20f * _rigidbody.mass));
        }
    }

    private void Start()
    {
        _rigidbody = entityPlayer.Rb;
        _playerTransform = entityPlayer.Hips;
        _joystickService = entityPlayer.JoystickService;
    }
}

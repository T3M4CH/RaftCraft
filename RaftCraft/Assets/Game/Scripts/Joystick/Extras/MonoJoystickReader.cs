using Game.Scripts.Joystick.Interfaces;
using Reflex.Attributes;
using UnityEngine;

public class MonoJoystickReader : MonoBehaviour
{
    private IJoystickService _joystickService;
    
    [Inject]
    private void Construct(IJoystickService joystickService)
    {
        _joystickService = joystickService;
    }

    private void Update()
    {
        print(_joystickService.Direction);
    }
}

using System;
using Game.Scripts.CameraSystem.Cameras;
using Game.Scripts.CameraSystem.Interfaces;
using Game.Scripts.Health.Interfaces;
using Game.Scripts.Player;
using Reflex.Attributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraExampleTest : MonoBehaviour
{
    private ICameraService _cameraService;
    private IHealthBarService _healthBarService;
    
    [Inject]
    public void Construct(ICameraService cameraService, IHealthBarService healthBarService)
    {
        _cameraService = cameraService;
        _healthBarService = healthBarService;
    }

    private void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            var player = FindObjectOfType<EntityPlayer>().transform;

            _cameraService.ChangeActiveCamera<SideVirtualCamera>().SetFollowAt(player).SetLookAt(player);
        }
    }
}

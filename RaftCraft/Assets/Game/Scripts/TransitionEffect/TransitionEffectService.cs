using System;
using Cinemachine;
using DG.Tweening;
using Game.Scripts.CameraSystem.Interfaces;
using Game.Scripts.TransitionEffect;
using Game.Scripts.TransitionEffect.Enums;
using Game.Scripts.TransitionEffect.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class TransitionEffectService : ITransitionEffectService, IDisposable
{
    private readonly ICameraService _cameraService;
    private readonly Image _image;

    public TransitionEffectService(ICameraService cameraService, SerializableTransitionSettings transitionSettings)
    {
        _cameraService = cameraService;
        _image = transitionSettings.DarkeningSprite;
    }

    public void ShowDarkening(float duration, EZoomType zoomType = EZoomType.None, float multiplier = 0)
    {
        var camera = _cameraService.GetActiveCamera();

        var transposer = camera.Camera.GetCinemachineComponent<CinemachineTransposer>();
        var offset = transposer.m_FollowOffset;

        offset *= multiplier;

        transposer.m_FollowOffset = offset;

        _image.DOColor(Color.black, duration);
    }

    public void Dispose()
    {
        _image.DOKill();
    }
}

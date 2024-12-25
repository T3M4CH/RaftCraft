using System;
using Game.Scripts.CameraSystem.Interfaces;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;

public class WaterDivingSystem : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Water")] private float yOffset;
    [SerializeField, FoldoutGroup("Water")] private float yMinValue;
    [SerializeField, FoldoutGroup("Water")] private Gradient _gradient;
    [SerializeField, FoldoutGroup("Water")] private MeshRenderer _renderer;
    [SerializeField, FoldoutGroup("Fade")] private float _maxScale;
    [SerializeField, FoldoutGroup("Fade")] private float _minScale;
    [SerializeField, FoldoutGroup("Fade")] private RectTransform _panelRect;
    [SerializeField, FoldoutGroup("Fade")] private RectTransform _fadeRectTransform;
    [SerializeField, FoldoutGroup("Fade")] private CanvasGroup _canvasGroup;
    [SerializeField] private Transform background;
    [SerializeField] private Transform _spriteTransform;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Camera _overlayCamera;
    
    private float _yMax;
    private float _zPosition;
    private Camera _mainCamera;
    private Transform _transform;
    private Transform _playerTransform;
    private IPlayerService _playerSpawner;
    private Material _backgroundMaterial;

    [Inject]
    private void Construct(IPlayerService playerSpawner, ICameraService cameraService)
    {
        _mainCamera = Camera.main;
        _mainCamera.GetUniversalAdditionalCameraData().cameraStack.Clear();
        _mainCamera.GetUniversalAdditionalCameraData().cameraStack.Add(_overlayCamera);
        //cameraService.SetOverlayCamera(_overlayCamera);
        _playerSpawner = playerSpawner;

        _playerSpawner.AddListener(ValidateNewPlayer);
    }

    private void ValidateNewPlayer(EPlayerStates state, EntityPlayer entityPlayer)
    {
        if (state != EPlayerStates.SpawnPlayer)
        {
            return;
        }
        _playerTransform = entityPlayer.transform;
    }

    private void LateUpdate()
    {
        if (_playerTransform == null) return;

        var newPosition = _playerTransform.position;

        var depthValue = Mathf.InverseLerp(_yMax, yMinValue, newPosition.y);

        newPosition.y = Mathf.Min(newPosition.y, _yMax) - yOffset;
        newPosition.z = _zPosition;

        background.localPosition = Vector3.Lerp(background.localPosition, new Vector3(0f,32f,3.5f), Time.deltaTime * 2);
        background.position = new Vector3(newPosition.x, Mathf.Min(-10, background.position.y),  background.position.z);

        _transform.position = newPosition;

        _canvasGroup.alpha = depthValue;
        var color = _gradient.Evaluate(depthValue);
        _backgroundMaterial.color = color;
        color.a = 255;
        _sprite.color = color;

        _fadeRectTransform.localScale = Vector3.one * Mathf.Lerp(_maxScale, _minScale, depthValue);
        _fadeRectTransform.localPosition = newPosition.WorldToScreenPosition(_mainCamera, _panelRect);

        var spritePosition = _spriteTransform.position;
        spritePosition.x = newPosition.x;

        _spriteTransform.position = spritePosition;
    }

    private void Start()
    {
        _transform = transform;
        _yMax = 0;
        _zPosition = _transform.position.z;
        _backgroundMaterial = _renderer.material;
    }

    private void OnDestroy()
    {
        _playerSpawner.RemoveListener(ValidateNewPlayer);
    }
}
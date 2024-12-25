using System;
using Game.Scripts.Player;
using Game.Scripts.Player.Damage;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.Spawners;
using Reflex.Attributes;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PressureController : MonoBehaviour
{
    [SerializeField] private float _delay;
    [SerializeField] private float _maxDamage;
    [SerializeField] private float _safetyDepth;
    [SerializeField] private float _startDamage;
    [SerializeField] private TMP_Text _depthValue;
    [SerializeField] private Image _depthIndicator;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Transform _waterBarrier;
    [SerializeField] private Transform _upgradeCanvas;
    [SerializeField] private TMP_Text upgradeText;
    
    private float _currentTime;
    private float _instantlyKillingDepth;
    private Material _depthMaterial;
    private Transform _playerTransform;
    private EntityPlayer _entityPlayer;
    private IPlayerService _playerSpawner;
    private WaterDamageController _waterDamageController;
    private IPlayerUpgradeSettings _playerUpgradeSettings;

    private const float Multiplier = 25;
    private readonly Color RedColor = new(1f, 0.17f, 0.16f);
    private readonly Color WhiteColor = new(0.97f, 1f, 1f);
    private static readonly int ScrollSpeed = Shader.PropertyToID("_ScrollSpeed");

    [Inject]
    private void Construct(IPlayerService playerSpawner)
    {
        _playerSpawner = playerSpawner;
        _playerSpawner.AddListener(ValidatePlayer);

        _playerUpgradeSettings = playerSpawner.UpgradeSettings;
        _playerUpgradeSettings.OnUpgrade += ValidateDepth;
        ValidateDepth(EPlayerUpgradeType.MaxDepth);
    }

    private void ValidateDepth(EPlayerUpgradeType type)
    {
        if (type == EPlayerUpgradeType.MaxDepth)
        {
            _safetyDepth = -_playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxDepth);
            var level = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.MaxDepth);
            if (level > 10)
            {
                upgradeText.text = "Unlock on \n Day 70";
            }
            var position = _waterBarrier.position;
            position.y = _safetyDepth;
            _waterBarrier.position = position;
            _instantlyKillingDepth = _safetyDepth - 200f;
        }
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        var canvasPos = _upgradeCanvas.position;
        canvasPos.x = _playerTransform.position.x;
        _upgradeCanvas.position = canvasPos;
        var yPos = _playerTransform.position.y;
        var value = Mathf.InverseLerp(0, _safetyDepth, yPos);

        _depthMaterial.SetFloat(ScrollSpeed, yPos / Multiplier);
        _depthValue.text = $"-{(int)Mathf.Max(-yPos,yPos)}m";
        if (yPos < _safetyDepth)
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > _delay)
            {
                _currentTime = 0;
                var depthValue = Mathf.InverseLerp(_safetyDepth, _instantlyKillingDepth, yPos);
                var damage = (int)Mathf.Lerp(_startDamage, _maxDamage, depthValue);

                _depthValue.color = _depthValue.color == RedColor ? WhiteColor : RedColor;
                _entityPlayer.TakeDamage(damage, Vector3.zero);
            }
        }
        else
        {
            _depthValue.color = WhiteColor;
            _currentTime = 0;
        }

        _canvasGroup.alpha = value == 0 ? 0 : 1;
    }

    private void ValidatePlayer(EPlayerStates state, EntityPlayer entityPlayer)
    {
        if (state != EPlayerStates.SpawnPlayer)
        {
            return;
        }

        _entityPlayer = entityPlayer;
        _playerTransform = _entityPlayer.transform;
    }

    private void Start()
    {
        _depthMaterial = new Material(_depthIndicator.material);
        _depthIndicator.material = _depthMaterial;
    }

    private void OnDestroy()
    {
        _playerUpgradeSettings.OnUpgrade -= ValidateDepth;
        _playerSpawner.RemoveListener(ValidatePlayer);
    }
}
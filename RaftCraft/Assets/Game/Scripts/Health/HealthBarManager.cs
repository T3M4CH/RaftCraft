using Game.Scripts.Health.Interfaces;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Health;
using System.Threading;
using Game.Scripts.NPC.Interface;
using Game.Scripts.Pool;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

public class HealthBarManager : IHealthBarService
{
    public HealthBarManager
    (
        //TODO: Нужно передавать игрока, чтобы получить уровень
        SerializableHealthBarSettings healthBarSettings
    )
    {
        _panel = healthBarSettings.Panel;
        _parent = healthBarSettings.Parent;
        _maxDistance = healthBarSettings.MaxDistance;
        _healthBarPrefab = healthBarSettings.HealthBarPrefab;
        _camera = Camera.main;
        _poolHealsBar = new PoolObjects<MonoHealthBar>(_healthBarPrefab, _parent, 10);
        ValidateHealthBarsPostLateUpdateLoop().Forget();
    }

    private bool _isDisposed;
    private float _heightMultiplier;

    private readonly float _maxDistance;
    private readonly Camera _camera;
    private readonly Transform _parent;
    private readonly RectTransform _panel;
    private readonly MonoHealthBar _healthBarPrefab;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Dictionary<Transform, MonoHealthBar> _healthBarsDictionary = new();

    private PoolObjects<MonoHealthBar> _poolHealsBar;

    public MonoHealthBar CreateHealthBar(Transform transform)
    {
        var healthBar = _poolHealsBar.GetFree();
        healthBar.gameObject.SetActive(true);
        healthBar.RectTransform.position = transform.position.WorldToScreenPosition(_camera, _panel);

        _healthBarsDictionary.Add(transform, healthBar);

        return healthBar;
    }

    public void SetLevel(Transform transform, int value)
    {
        _healthBarsDictionary.TryGetValue(transform, out var healthBar);

        if (!healthBar)
        {
            healthBar = CreateHealthBar(transform);
        }

        healthBar.SetLevel(value + 1, Color.green);
    }

    public void SetValue(Transform transform, IHeals value)
    {
        _healthBarsDictionary.TryGetValue(transform, out var healthBar);

        if (!healthBar)
        {
            healthBar = CreateHealthBar(transform);
        }

        healthBar.SetTarget(value);
    }

    public void RemovePanel(Transform transform)
    {
        _healthBarsDictionary.TryGetValue(transform, out var healthBar);

        if (!healthBar) return;
        healthBar.gameObject.SetActive(false);
        _healthBarsDictionary.Remove(transform);
    }

    private async UniTaskVoid ValidateHealthBarsPostLateUpdateLoop()
    {
        while (!await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, _panel.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
        {
            foreach (var pair in _healthBarsDictionary)
            {
                var targetPosition = pair.Key.position + Vector3.up;
                pair.Value.RectTransform.localPosition = targetPosition.WorldToScreenPosition(_camera, _panel);
                pair.Value.RectTransform.localScale = Vector3.one;
            }
        }
    }
}
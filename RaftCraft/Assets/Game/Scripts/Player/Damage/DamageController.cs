using System;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Sirenix.OdinInspector;
using Game.Scripts.Player;
using Game.Scripts.NPC;
using Game.Scripts.Player.HeroPumping.Enums;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    //TODO: Заменить в будущем на отдельный класс с пулом всех эффектов
    [SerializeField, FoldoutGroup("DeathInWater")] private GameObject _poofEffect;
    [SerializeField, FoldoutGroup("DeathOnGround")] private Animator _animator;
    [SerializeField, FoldoutGroup("DeathOnGround")] private Collider _mainCollider;
    [SerializeField, FoldoutGroup("DeathOnGround")] private MonoRagdollFragment[] _fragments;

    [SerializeField] private EntityPlayer _player;
    [SerializeField] private PlayerHealthBar _healthBar;

    private float _maxHealth;
    public float CurrentHealth { get; private set; }

    private void ValidateHealth(IDamagable _, float damage)
    {
        if (CurrentHealth <= 0) return;

        var convertedDamage = (float)damage;

        CurrentHealth -= convertedDamage;
        CurrentHealth = Mathf.Min(CurrentHealth, _maxHealth);

        if (CurrentHealth <= 0)
        {
            if (_player.StateMachine.CurrentEntityState.GetType() == typeof(PlayerWaterState))
            {
                _player.StateMachine.SetState<PlayerDeathInWater>();
            }
            else
            {
                _player.StateMachine.SetState<PlayerDeathInGround>();
            }
        }

        _healthBar.SetValue(CurrentHealth / _maxHealth);
        _healthBar.SetValueTextHeals((int)CurrentHealth);
    }

    private void ValidateHealth(EPlayerUpgradeType upgradeType)
    {
        _maxHealth = _player.PlayerSettings.GetValue<float>(EPlayerUpgradeType.MaxHealth);
        CurrentHealth = _maxHealth;
        _healthBar.SetValue(CurrentHealth / _maxHealth);
        _healthBar.SetValueTextHeals((int)CurrentHealth);
    }
    
    private void OnDestroy()
    {
        _player.PlayerSettings.OnUpgrade -= ValidateHealth;
    }

    private void PlayerOnOnHeal()
    {
        ValidateHealth(EPlayerUpgradeType.MaxHealth);
    }
}
using System;
using Game.Scripts.NPC;
using Game.Scripts.Player.EntityGame;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.BulletSystem
{
    public class Bullet : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody _rb;
        [SerializeField] private BulletSettings _settings;
        
        private Vector3 _targetDirection;
        private float _timeLife;
        private EntityType _targetLayer;
        private float _damage;

        private bool _critical = false;
        
        public void StartMove(Vector3 direction, EntityType target, float damage, bool critical = false)
        {
            _critical = critical;
            _targetLayer = target;
            _targetDirection = direction;
            _timeLife = _settings.TimeLife;
            _damage = damage;
        }

        private void FixedUpdate()
        {
            _rb.MovePosition(transform.position + (_targetDirection * (_settings.SpeedMove * Time.fixedDeltaTime)));
            _timeLife -= Time.smoothDeltaTime;
            if (_timeLife <= 0f)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamagable>(out var damage))
            {
                if (_targetLayer.HasFlag(damage.CurrentType) == false)
                {
                    return;
                }
                damage.TakeDamage(_damage, transform.position, _critical);
                gameObject.SetActive(false);
            }
        }
    }
}

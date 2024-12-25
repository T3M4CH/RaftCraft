using System;
using UnityEngine;

namespace Game.Scripts.Player.Damage
{
    public class WaterDamageController : MonoBehaviour
    {
        [SerializeField] private float _safetyDepth;
        [SerializeField] private float _instantlyKillingDepth;
        [SerializeField] private float _startDamage;
        [SerializeField] private float _maxDamage;
        [SerializeField] private float _delay;
        [SerializeField] private EntityPlayer player;

        private float _currentTime;
        private Transform _transform;

        private void Update()
        {
            var yPos = _transform.position.y;
            if (yPos < _safetyDepth)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime > _delay)
                {
                    _currentTime = 0;
                    var value = Mathf.InverseLerp(_safetyDepth, _instantlyKillingDepth, yPos);
                    var damage = (int)Mathf.Lerp(_startDamage, _maxDamage, value);
                    
                    player.TakeDamage(damage, Vector3.zero);
                }
            }
            else
            {
                _currentTime = 0;
            }
        }

        private void Start()
        {
            _transform = transform;
        }
    }
}
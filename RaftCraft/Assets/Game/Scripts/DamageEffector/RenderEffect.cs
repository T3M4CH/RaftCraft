using System;
using Game.Scripts.NPC;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.DamageEffector
{
    public class RenderEffect : MonoBehaviour
    {
        [SerializeField] private MMF_Player _player;
        [SerializeField] private OutlineFlicker _flickerOutline;

        private MMF_Flicker _flicker;
        
        private IDamagable _damagable;

        private void OnEnable()
        {
            _damagable = GetComponent<IDamagable>();
            if (_damagable == null)
            {
                Destroy(this);
                return;
            }

            _flicker ??= _player.GetFeedbackOfType<MMF_Flicker>();
            _damagable.OnDamage += DamagableOnOnDamage;
        }

        private void DamagableOnOnDamage(IDamagable arg1, float damage)
        {
            Debug.Log($"On event damage effect");
            _player.Initialization();
            _player.PlayFeedbacks();
            _flickerOutline.PlayEffect();
        }

        [Button]
        private void Test()
        {
            _player.Initialization();
            _player.PlayFeedbacks();
            _flickerOutline.PlayEffect();
        }
        

        private void OnDisable()
        {
            if (_damagable == null)
            {
                return;
            }

            _damagable.OnDamage -= DamagableOnOnDamage;
        }
    }
}

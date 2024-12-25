using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleController : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> _particle = new List<ParticleSystem>();

        private ParticleSystem _particleSystem;
        private ParticleSystem.MainModule _main;

        public void Awake()
        { 
            _particleSystem = GetComponent<ParticleSystem>();
            _main = _particleSystem.main;
            _main.stopAction = ParticleSystemStopAction.Callback;
        }

        public void SetColorLifeTime(Color color)
        {
            foreach (var particle in _particle)
            {
                var col = particle.colorOverLifetime;
                Gradient grad = new Gradient();
                grad.SetKeys( new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0f, 1.0f) } );
                col.color = grad;
            }
        }

        public void OnEnable()
        { 
            _particleSystem.Play();
        }
    
        private void OnParticleSystemStopped()
        {
            gameObject.SetActive(false);
        }
    }
}
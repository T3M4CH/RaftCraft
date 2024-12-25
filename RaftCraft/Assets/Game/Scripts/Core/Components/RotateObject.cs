using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core.Components
{
    public class RotateObject : MonoBehaviour
    {
        [SerializeField] private Vector3 angle;
        [SerializeField] private bool _isRandom;

        private void Awake()
        {
            if (_isRandom)
            {
                angle *= Random.Range(-1f, 1f);
            }
        }

        private void Update()
        {
            transform.Rotate(angle * Time.fixedDeltaTime);
        }
    }
}
    
    

using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Tutorial.Old
{
    public class LineRendererMove : MonoBehaviour
    {
        [SerializeField] private LineRenderer _renderer;
        [SerializeField] private float _positionZ;

        [SerializeField] private float _offset;
        
        private Material _material;
        [SerializeField, ReadOnly]
        private Vector2 _tiling;

        public int CountPoint
        {
            get => _renderer.positionCount;
            set => _renderer.positionCount = value;
        }

        public void SetStateLine(bool state)
        {
            _renderer.enabled = state;
        }
        
        public void SetPosition(int index, Vector3 position)
        {
            position.z = _positionZ;
            var direction = Vector3.zero;
            switch (index)
            {
                case 0:
                    direction = (_renderer.GetPosition(1) - position).normalized;
                    break;
                case 1:
                    direction = (_renderer.GetPosition(0) - position).normalized;
                    _material.mainTextureScale = new Vector2(Vector3.Distance(_renderer.GetPosition(0), position + (direction * _offset)), 1f);
                    break;
            }
            
            _renderer.SetPosition(index, position + (direction * _offset));
        }
        private void Awake()
        {
            _material = new Material(_renderer.sharedMaterial);
            _renderer.sharedMaterial = _material;
        }
        

        private void Update()
        {
            _tiling.x -= Time.smoothDeltaTime;
            _material.mainTextureOffset = _tiling;
        }
    }
}

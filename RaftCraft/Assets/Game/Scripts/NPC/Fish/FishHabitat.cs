using UnityEngine;

namespace Game.Scripts.NPC
{
    public class FishHabitat : MonoBehaviour
    {
        [SerializeField] private float _widthBorder = 9f;
        [SerializeField] private float _heightBorder = 5f;
        //[SerializeField] private float _heightSpread = 1f;
        
        [Header("Settings draw points")]
        [SerializeField] private Color _color;

        private Vector3 _p1, _p2, _p3, _p4;

        public Vector3 GetNewTargetPoint(Vector3 from, float? yMin = null, float? yMax = null)
        {
            var result = from;
            result.x = transform.position.x + Random.Range(-_widthBorder, _widthBorder);
            result.y = transform.position.y + Random.Range(-_heightBorder, _heightBorder);
            
            if (yMin.HasValue && yMax.HasValue)
            {
                result.y = Random.Range(yMin.Value, yMax.Value);
            }
            //result.x = (from.x + Random.Range(_widthBorder * 0.8f, _widthBorder * 1.8f) - _p1.x) % (_widthBorder * 2f) + _p1.x;
            return result;
        }

        public Vector3 ClampTargetPosition(Vector3 targetPosition)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, -_widthBorder, _widthBorder);
            targetPosition.y = Mathf.Clamp(targetPosition.y, -_heightBorder, _heightBorder);
            return targetPosition;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() 
        {
            Gizmos.color = _color;
            
            _p1 = transform.position;
            _p2 = transform.position;
            _p3 = transform.position;
            _p4 = transform.position;
            
            _p1.x -= _widthBorder;
            _p1.y += _heightBorder;
            
            _p2.x += _widthBorder;
            _p2.y += _heightBorder;
            
            _p3.x += _widthBorder;
            _p3.y -= _heightBorder;
            
            _p4.x -= _widthBorder;
            _p4.y -= _heightBorder;
            
            Gizmos.DrawLine(_p1, _p2);
            Gizmos.DrawLine(_p2, _p3);
            Gizmos.DrawLine(_p3, _p4);
            Gizmos.DrawLine(_p4, _p1);
        }
#endif
    }
}
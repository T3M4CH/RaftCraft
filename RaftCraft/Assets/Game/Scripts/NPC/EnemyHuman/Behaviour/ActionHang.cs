using UnityEngine;

namespace Game.Scripts.NPC
{
    public class ActionHang: IAction
    {
        private Vector3 _target;
        private readonly Transform _transform;
        private readonly float _speed;

        public ActionHang(Vector3 target, Transform transform, float speed)
        {
            _target = target;
            _transform = transform;
            _speed = speed;
        }
        
        public bool Execute()
        {
            _transform.position = Vector3.MoveTowards(_transform.position, _target, Time.deltaTime * _speed);
            
            return Vector3.Distance(_transform.position, _target) < 0.1f;
        }
    }
}
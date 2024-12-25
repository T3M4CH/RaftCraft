using Game.Scripts.Player.Collision;
using UnityEngine;

namespace Game.Scripts.Player.EntityGame
{
    [System.Serializable]
    public struct EntityMonoComponents
    {
        [SerializeField] private Transform _parentObject;
        [SerializeField] private Transform _modelObject;
        [SerializeField] private EntityCollision _collision;

        public Transform ParentObject => _parentObject;
        public Transform ModelObject => _modelObject;

        public EntityCollision Collision => _collision;
    }
}

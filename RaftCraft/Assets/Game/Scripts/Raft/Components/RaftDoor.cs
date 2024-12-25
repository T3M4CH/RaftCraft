using UnityEngine;

namespace Game.Scripts.Raft.Components
{
    public class RaftDoor : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        
        public void DeadTile(Vector3 direction)
        {
            _collider.enabled = false;
        }

        public void Restore()
        {
            _collider.enabled = true;
        }
    }
}

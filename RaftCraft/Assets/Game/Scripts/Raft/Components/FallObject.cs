using Game.Scripts.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Raft.Components
{
    public class FallObject : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Settings")] private LayerMask _maskEnter;
        [SerializeField, FoldoutGroup("Settings")] private LayerMask _maskExit;
        
        public void Enter(EntityPlayer player)
        {
            player.Rb.excludeLayers = _maskEnter;
        }

        public void Exit(EntityPlayer player)
        {
            player.Rb.excludeLayers = _maskExit;
        }
    }
}

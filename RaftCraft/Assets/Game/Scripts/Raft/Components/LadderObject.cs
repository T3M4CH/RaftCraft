using Game.Scripts.Raft.BuildSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Raft.Components
{
    public class LadderObject : MonoBehaviour
    {
        [SerializeField] private TileBuild _tileRaft;

        public bool HaveInteraction => _tileRaft.GlobalState == BuildState.UnLock;
        
        [field: SerializeField, FoldoutGroup("Settings")] public Transform StartPoint { get; private set; }
        [field: SerializeField, FoldoutGroup("Settings")] public Transform EndPoint { get; private set; }
        
    }
}
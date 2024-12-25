using Game.Scripts.Raft.BuildSystem;
using UnityEngine;

public class GrabPoint : MonoBehaviour
{
    [SerializeField] private TileBuild _tileRaft;
    [SerializeField] private bool _isLeft;
    
    public bool HaveInteraction => _tileRaft.GlobalState == BuildState.UnLock;
    public Vector3 Position => transform.position;
    public bool IsLeft => _isLeft;
}

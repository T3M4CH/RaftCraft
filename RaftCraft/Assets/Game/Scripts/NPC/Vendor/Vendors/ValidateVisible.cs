using Cysharp.Threading.Tasks;
using Game.Scripts.Raft.Interface;
using Reflex.Attributes;
using UnityEngine;

public class ValidateVisible : MonoBehaviour
{
    [SerializeField] private int _row;
    private IRaftStructures _raftStructures;

    [Inject]
    private void Init(IRaftStructures raftStructures)
    {
        _raftStructures = raftStructures;
        _raftStructures.OnUpdateRaftStruct += UpdateState;
    }

    private async void UpdateState()
    {
        await UniTask.DelayFrame(1);
        transform.localScale = _raftStructures.CheckUnlockRow() >= _row ? Vector3.one : Vector3.zero;
    }

    private void OnDestroy()
    {
        _raftStructures.OnUpdateRaftStruct -= UpdateState;
    }
}

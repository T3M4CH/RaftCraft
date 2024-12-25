using System;
using Game.Scripts.Raft.BuildSystem;
using Game.Scripts.Raft.Components;
using UnityEngine;

namespace Game.Scripts.Raft.Interface
{
    public interface IRaftStructures
    {
        TileBuild GetActiveTile(out int row, out int column);
        public Vector3 GetLeftRaftPosition();
        public Vector3 GetRightRaftPosition();
        public LadderObject[] GetStairs();
        public GrabPoint[] GetGrabPoints();
        public TileEntity[] GetDoors();
        public int CheckUnlockRow();
        public float ClampPositionHorizontal(Vector3 position, float direction, float offset);
        public Vector2 OffsetHorizontal { get; set; }
        public event Action OnUpdateRaftStruct;
    }
}
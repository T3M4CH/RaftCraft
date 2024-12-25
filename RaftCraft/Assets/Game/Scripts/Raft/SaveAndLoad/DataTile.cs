using System.Collections.Generic;
using Game.Scripts.Raft.BuildSystem;

namespace Game.Scripts.Raft.SaveAndLoad
{
    [System.Serializable]
    public class DataTile
    {
        public string GUID;
        public BuildState StateBuild;
        public int CountResource;
        public float TimeBuild;
    }

    [System.Serializable]
    public class DataRow
    {
        public string GUID;
        public RowState State;
        public List<DataTile> DataTiles;
    }
}

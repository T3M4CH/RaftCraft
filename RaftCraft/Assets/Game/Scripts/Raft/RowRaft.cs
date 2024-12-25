using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.Raft.BuildSystem;
using Game.Scripts.Raft.SaveAndLoad;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.Scripts.Raft
{
    public enum RowState
    {
        Lock,
        ViewBuild,
        UnLock
    }
    
    public class RowRaft : MonoBehaviour
    {
        [System.Serializable]
        public class TileQueue
        {
            [field: SerializeField]
            public TileBuild Current { get; private set; }
            [field: SerializeField]
            public TileBuild Next { get; private set; }
            [field: SerializeField]
            public bool PreloadTwoNext { get; private set; }
        }
        public event Action<RowRaft> OnFullBuild; 
        
        [field: SerializeField, ReadOnly] public string ID { get; private set; }
        [SerializeField, FoldoutGroup("Settings")] private RowState _preloadState;
        [field: SerializeField, OnValueChanged("UpdatePosition"), FoldoutGroup("Settings")] public Vector3 Offset { get; private set; }
        [field: SerializeField, FoldoutGroup("Queues")] public List<TileQueue> Queues { get; private set; }
        [field: SerializeField, FoldoutGroup("Constructor"), ReadOnly] public List<TileRaftBuild> Tiles { get; private set; }

        [field: SerializeField, FoldoutGroup("Constructor"), ReadOnly] public RaftConstructor Constructor { get; private set; }
        [field: SerializeField, FoldoutGroup("Constructor")] public RowRaft PreviewRow { get; private set; }
        [field: SerializeField, FoldoutGroup("Constructor")] public List<GameObject> LockObjects { get; private set; }

        [SerializeField, FoldoutGroup("Feedback Lock Object")] private float _durationShow;
        [SerializeField, FoldoutGroup("Feedback Lock Object")] private Ease _easeShow;

        private RowState _state;

        [ShowInInspector, ReadOnly, FoldoutGroup("Debug")]
        public RowState State
        {
            get => _state;

            private set
            {
                _state = value;
                switch (_state)
                {
                    case RowState.Lock:
                        break;
                    case RowState.ViewBuild:
                        break;
                    case RowState.UnLock:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public void Init(RaftConstructor constructor)
        {
            Constructor = constructor;
            Tiles = new List<TileRaftBuild>();
#if UNITY_EDITOR
            ID = GUID.Generate().ToString();
#endif
        }
        
        public void AddTile(TileRaftBuild tileRaft)
        {
            if (Tiles.Contains(tileRaft))
            {
                return;
            }
            
            Tiles.Add(tileRaft);
            tileRaft.ConstructorTile.transform.SetParent(transform);
        }

        public void Initialize()
        {
            foreach (var tile in Tiles)
            {
                tile.Build.OnStateChanged += BuildOnOnStateChanged;
            }

            if (PreviewRow != null)
            {
                PreviewRow.OnFullBuild += PreviewRowOnOnFullBuild;
            }
        }

        public void Load(DataRow data = null)
        {
            if (data == null)
            {
                foreach (var tile in Tiles)
                {
                    tile.Build.LoadDefault();
                }

                State = _preloadState;
                switch (State)
                {
                    case RowState.Lock:
                        SetStateLockObjects(false);
                        break;
                    case RowState.ViewBuild:
                        SetStateLockObjects(true);
                        break;
                    case RowState.UnLock:
                        SetStateLockObjects(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                return;
            }
            
            State = data.State;
            switch (State)
            {
                case RowState.Lock:
                    SetStateLockObjects(false);
                    break;
                case RowState.ViewBuild:
                    SetStateLockObjects(true);
                    break;
                case RowState.UnLock:
                    SetStateLockObjects(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (data.DataTiles == null || data.DataTiles.Count == 0)
            {
                foreach (var tile in Tiles)
                {
                    tile.Build.LoadDefault();
                }
            }
            else
            {
                foreach (var tile in data.DataTiles)
                {
                    var loadTile = GetTileById(tile.GUID);
                    if (loadTile == null)
                    {
                        Debug.Log($"Tile: {tile.GUID} not found!");
                        continue;
                    }
                    loadTile.Build.SetLoadedData(tile);
                }

                foreach (var tile in Tiles)
                {
                    //tile.Build.SetStateFazeTow(HaveFloorUnLocked());
                    //tile.Build.SetStateFazeTow(PreviewRow == null ? HaveFloorUnLocked() : PreviewRow.HaveFloorUnLocked());
                }
            }

           
        }

        private void SetStateLockObjects(bool state)
        {
            foreach (var obj in LockObjects)
            {
                if (obj == null)
                {
                    continue;
                }
                
                obj.SetActive(state);
            }
        }

        private void PlayAnimationShowLock()
        {
            foreach (var obj in LockObjects)
            {
                obj.transform.DOKill();
                obj.SetActive(true);
                obj.transform.localScale = Vector3.zero;
                obj.transform.DOScale(Vector3.one, _durationShow).SetEase(_easeShow);
            }
        }
        
        private TileRaftBuild GetTileById(string guid)
        {
            foreach (var tile in Tiles)
            {
                if (tile.ID == guid)
                {
                    return tile;
                }
            }

            return null;
        }

        private void PreviewRowOnOnFullBuild(RowRaft preview)
        {
            BuildTilesFazeTwo();
            if (Queues == null || Queues.Count == 0)
            {
                State = RowState.UnLock;
                return;
            }

            State = RowState.ViewBuild;
            PlayAnimationShowLock();
            var next = Queues[0];
            next.Current.UnLockBuild();
        }

        private void BuildTilesFazeTwo()
        {
            if (Queues != null && Queues.Count > 0)
            {
                Queues[0].Current.BuildFazeTow();
                return;
            }
        }
        
        private void UpdatePosition()
        {
            transform.localPosition = Offset;
        }
        
        private void BuildOnOnStateChanged(BuildState state, TileBuild tile)
        {
            if (Queues == null || Queues.Count == 0)
            {
                if (PreviewRow != null && PreviewRow.HaveFloorUnLocked())
                {
                    Debug.Log($"Set state row: Почему бля? {PreviewRow}:{PreviewRow.HaveFloorUnLocked()}");
                    State = RowState.UnLock;
                    return;
                }
                
                return;
            }
            if (state == BuildState.UnLock)
            {
                if (HaveFloorUnLocked())
                {
                    BuildTilesFazeTwo();
                    State = RowState.UnLock;
                    OnFullBuild?.Invoke(this);
                    SetStateLockObjects(false);
                    return;
                }
                
                var next = GetQueue(tile);
                if (next == null)
                {
                    return;
                }

                if (next.Next == null)
                {
                    BuildTilesFazeTwo();
                    State = RowState.UnLock;
                    SetStateLockObjects(false);
                    OnFullBuild?.Invoke(this);
                    return;
                }

                if (next.PreloadTwoNext)
                {
                    next.Next.BuildFazeTow();
                }
                next.Next.UnLockBuild();
            }
        }

        private TileQueue GetQueue(TileBuild target)
        {
            foreach (var queue in Queues)
            {
                if (queue.Current == target && queue.Next != target)
                {
                    return queue;
                }   
            }

            return null;
        }
        
        public bool HaveFloorUnLocked()
        {
            foreach (var tile in Tiles)
            {
                if (tile.Build.State != BuildState.UnLock)
                {
                    return false;
                }
            }

            return true;
        }

        public List<DataTile> GetDataTiles()
        {
            var result = new List<DataTile>();
            foreach (var tile in Tiles)
            {
                result.Add(new DataTile()
                {
                    GUID = tile.ID,
                    StateBuild = tile.Build.State,
                    CountResource = tile.Build.CountResource,
                    TimeBuild = tile.Build.Timer
                });
            }

            return result;
        }
        
        [Button, FoldoutGroup("Constructor")]
        private void Remove()
        {
#if UNITY_EDITOR
            Constructor.RemoveRow(this);
#endif
        }

        private void OnDisable()
        {
            if (PreviewRow != null)
            {
                PreviewRow.OnFullBuild += PreviewRowOnOnFullBuild;
            }
            
            foreach (var tile in Tiles)
            {
                tile.Build.OnStateChanged -= BuildOnOnStateChanged;
            }
        }
        
    }
}

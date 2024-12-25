using System;
using System.Collections.Generic;
using Game.Scripts.Days;
using Game.Scripts.GameIndicator;
using Game.Scripts.Raft.BuildSystem;
using Game.Scripts.Raft.Components;
using Game.Scripts.Raft.Interface;
using Game.Scripts.Raft.SaveAndLoad;
using Game.Scripts.Saves;
using Reflex.Attributes;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.Scripts.Raft
{
    
    [System.Serializable]
    public class TileRaftBuild
    {
        [field: SerializeField]
        public Vector3 Position { get; private set; }

        [field: SerializeField, FoldoutGroup("Constructor"), ReadOnly] public TileConstructor ConstructorTile { get; private set; }
        [field: SerializeField, FoldoutGroup("Constructor"), ReadOnly] public TileBuild Build { get; private set; }

        [field: SerializeField, FoldoutGroup("Save And Load"), ReadOnly]
        public string ID { get; private set; }

        public TileRaftBuild(Vector3 position)
        {
            Position = position;
            ID = System.Guid.NewGuid().ToString();
        }

        public TileRaftBuild Construct(TileConstructor constructor, TileBuild build)
        {
            ConstructorTile = constructor;
            Build = build;
            return this;
        }
            
        public void UpdatePosition(Vector3 offset)
        {
            ConstructorTile.UpdatePosition(Position, offset);
        }
    }
    
    public class RaftConstructor : MonoBehaviour, IRaftStructures
    {
        [System.Serializable]
        public class BuildInDay
        {
            public int DayIndex;
            public TileBuild Build;
            public int CostBuild;
        }

        [SerializeField, FoldoutGroup("Settings")]
        private Transform _contentTiles;

        [SerializeField, FoldoutGroup("Settings")]
        private List<BuildInDay> _buildInDays = new List<BuildInDay>();

        [SerializeField, FoldoutGroup("Settings")]
        private TileConstructor _prefabTile;

        [SerializeField, FoldoutGroup("Settings")]
        private Vector2Int _sizeRaft;

        [SerializeField, FoldoutGroup("Settings")]
        private Vector2 _sizeTile;


        [SerializeField, FoldoutGroup("Construct")]
        private List<RowRaft> _rows = new List<RowRaft>();
        
        
        private IDayService _dayController;
        private GameSave _gameSave;
        
        public event Action OnUpdateRaftStruct;

        [Inject]
        private void Construct(IControllerIndicator indicatorController, IDayService dayController, GameSave gameSave)
        {
            var icon = Resources.Load<IconConfig>("IconConfig").HomeIcon;
            indicatorController.AddTarget(transform, new Vector3(Screen.width / 2f,Screen.height - 200,0), 30, icon,new Color32(255, 202, 0, 255), true);
            _gameSave = gameSave;
            _dayController = dayController;
            _dayController.OnDayStart += DayControllerOnOnDayStart;
        }

        private void Start()
        { 
            foreach (var row in _rows)
            {
                row.Initialize();
            }
            
            LoadData();
        }

        private void OnEnable()
        {
            foreach (var row in _rows)
            {
                foreach (var tile in row.Tiles)
                {
                    tile.Build.OnBuyTile += BuildOnOnBuyTile;
                }
            }
        }

        public TileBuild GetActiveTile(out int row, out int column)
        {
            row = 0;
            column = 0;
            foreach (var rowRaft in _rows)
            {
                row += 1;
                var tiles = rowRaft.Tiles;
                for (var i = 0; i < tiles.Count; i++)
                {
                    column = i;
                    if (tiles[i].Build.Interaction)
                    {
                        return tiles[i].Build;
                    }
                }
            }
            
            throw new Exception("No active tile");
        }

        private void BuildOnOnBuyTile()
        {
            OnUpdateRaftStruct?.Invoke();
            SaveData();
        }

        private void OnDestroy()
        {
            SaveData();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveData();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus == false)
            {
                SaveData();
            }
        }

        private void DayControllerOnOnDayStart(int dayId)
        {
            foreach (var build in _buildInDays)
            {
                if (build.DayIndex <= dayId && build.Build.State == BuildState.Lock)
                {
                    
                    //TODO убрать логику автоматического определения стороны строительства
                   // build.Build.SetCost(build.CostBuild);
                    //build.Build.SetStateVisibleBuild(true);
                }
            }
        }

        private void LoadData()
        {
            var rowPresets = new Dictionary<string, RowRaft>();
            foreach (var row in _rows)
            {
                rowPresets.TryAdd(row.ID, row);
            }
            
            var rowData = _gameSave.GetData(SaveConstants.RaftKey, new Dictionary<string, DataRow>());
            foreach (var preset in rowPresets)
            {
                if (rowData.ContainsKey(preset.Key))
                {
                    preset.Value.Load(rowData[preset.Key]);
                }
                else
                {
                    preset.Value.Load();
                }
            }
        }
        
        [Button]
        public void SaveData()
        {
            //TODO: Множественные условия
            if(!_gameSave.HaveKey(SaveConstants.TutorialOne)) return;
            
            var data = new Dictionary<string, DataRow>();
            foreach (var row in _rows)
            {
                data.TryAdd(row.ID, new DataRow()
                {
                    GUID = row.ID,
                    State = row.State,
                    DataTiles = row.GetDataTiles()
                });
            }
            _gameSave.SetData(SaveConstants.RaftKey, data);
        }
        

#if UNITY_EDITOR
        [Button]
        private void AddRow(int size)
        {
            var startPosition = new Vector2(0f, _sizeTile.y * _rows.Count);
            var row = new GameObject($"Row_{_rows.Count}").AddComponent<RowRaft>();
            row.Init(this);
            row.transform.SetParent(_contentTiles);
            for (var i = 0; i < size; i++)
            {
                var tile = new TileRaftBuild(startPosition);
                var instanceTile = PrefabUtility.InstantiatePrefab(_prefabTile) as TileConstructor;
                instanceTile.UpdateStruck();
                instanceTile.Build.gameObject.name = $"Tile_{_rows.Count + i}";
                instanceTile.transform.localPosition = tile.Position;
                tile = tile.Construct(instanceTile, instanceTile.Build);
                row.AddTile(tile);
                startPosition.x += _sizeTile.x;
            }
            
            _rows.Add(row);
        }

        public void RemoveRow(RowRaft row)
        {
#if UNITY_EDITOR
            _rows.Remove(row);
            DestroyImmediate(row.gameObject);
#endif
        }
#endif

        public Vector3 GetLeftRaftPosition()
        {
            return GetLastTile(true).ConstructorTile.transform.position - new Vector3(_sizeTile.x * 0.5f + OffsetHorizontal.x, 0f);
        }

        public Vector3 GetRightRaftPosition()
        {
            return GetLastTile(false).ConstructorTile.transform.position + new Vector3(_sizeTile.x * 0.5f + OffsetHorizontal.y, 0f);
        }

        public int CheckUnlockRow()
        {
            for (int i = 1; i < _rows.Count; i++)
            {
                if (_rows[i].State != RowState.UnLock)
                {
                    return i - 1;
                }
            }

            return _rows.Count - 1;
        }
        
        public GrabPoint[] GetGrabPoints()
        {
            return _rows.Count > 2 ? _rows[2].GetComponentsInChildren<GrabPoint>(true) : null;
        }
        
        public LadderObject[] GetStairs()
        {
            return GetComponentsInChildren<LadderObject>();
        }
        
        public TileEntity[] GetDoors()
        {
            return GetComponentsInChildren<TileEntity>();
        }

        private TileRaftBuild GetLastTile(bool left)
        {
            for (var i = left ? 0 : _rows[0].Tiles.Count - 1;
                 left ? i < _rows[0].Tiles.Count : i >= 0;
                 i = left ? i += 1 : i -= 1)
            {
                if (_rows[0].Tiles[i].ConstructorTile.Build.HaveConstruct)
                {
                    return _rows[0].Tiles[i];
                }
            }

            return null;
        }

        private float GetValueMax()
        {
            return -(_sizeTile.x / 2f * ((_sizeRaft.y / 2) + 1));
        }

        private float GetValueMin()
        {
            return -(_sizeTile.x * (_sizeRaft.y / 2));
        }


        [Button]
        private void Clear()
        {
            foreach (var row in _rows)
            {
#if UNITY_EDITOR
                DestroyImmediate(row.gameObject);
#endif
            }

            _rows = new List<RowRaft>();
        }
        
        public float ClampPositionHorizontal(Vector3 position, float direction, float offset)
        {
            var result = direction;
            if (position.x >= (GetRightRaftPosition().x - offset))
            {
                result = Mathf.Clamp(result, -1f, 0f);
            }

            if (position.x <= (GetLeftRaftPosition().x + offset))
            {
                result = Mathf.Clamp(result, 0f, 1f);
            }

            return result;
        }

        public Vector2 OffsetHorizontal { get; set; }

        private void OnDisable()
        {
            if (_dayController != null)
            {
                _dayController.OnDayStart -= DayControllerOnOnDayStart;
            }
            
            foreach (var row in _rows)
            {
                foreach (var tile in row.Tiles)
                {
                    tile.Build.OnBuyTile -= BuildOnOnBuyTile;
                }
            }
        }
    }
}
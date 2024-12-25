using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Raft.BuildSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Raft
{
    public class TileConstructor : MonoBehaviour
    {    
        [field: SerializeField] public List<TileRaft> Tiles { get; private set; }
        [field: SerializeField] public TileBuild Build { get; private set; }
        
        
        [field: SerializeField, FoldoutGroup("Constructor"), OnValueChanged("UpdateStruck")]
        public PartTile Parts { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Constructor"), OnValueChanged("UpdatePositionBuild")]
        public Vector3 PositionBuild { get; private set; }

        [field: SerializeField, FoldoutGroup("Constructor"), OnValueChanged("UpdateViewBig")]
        public bool IsShowBigPanel { get; private set; }

        [Button]
        private void GatParts()
        {
            Tiles = GetComponentsInChildren<TileRaft>().ToList();
        }

        public void UpdatePosition(Vector3 start, Vector3 offset)
        {
            transform.localPosition = start + offset;
        }

        public void UpdatePositionBuild()
        {
            Build.UpdatePositionBuild(PositionBuild);
        }

        public void UpdateViewBig()
        {
            Build.UpdateStateBigPanel(IsShowBigPanel);
        }
        
        public void UpdateStruck()
        {
            foreach (var tile in Tiles)
            {
                if (Parts.HasFlag(tile.Part))
                {
                    Build.AddTile(tile);
                    tile.gameObject.SetActive(true);
                }
                else
                {
                    Build.RemoveTile(tile);
                    tile.gameObject.SetActive(false);
                }
            }
        }
    }

    [System.Flags]
    public enum PartTile
    {
        BeamsLeft = 1 << 0,
        BeamsRight = 1 << 1,
        DoorLeft = 1 << 2,
        DoorRight = 1 << 3,
        DoorWayLeft = 1 << 4,
        DoorWayRight = 1 << 5,
        RaftDown = 1 << 6,
        RaftTop = 1 << 7,
        RaftFishingFront = 1 << 8,
        RaftFishingLeft = 1 << 9,
        RaftFishingRight = 1 << 10,
        LadderFrontAndDescent = 1 << 11,
        LadderLeftAndDescent = 1 << 12,
        LadderRightAndDescent = 1 << 13,
        LadderFront = 1 << 14,
        LadderLeft = 1 << 15,
        LadderRight = 1 << 16,
        WallOne = 1 << 17,
        WallTwo = 1 << 18,
        WallThree = 1 << 19,
        WallFour = 1 << 20,
        WallFive = 1 << 21 
    }
}

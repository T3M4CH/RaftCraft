using Game.Scripts.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Player.PlayerController
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Game/Player/Settings")]
    public class EntityPlayerSettings : ScriptableObject, IWindowObject
    {
        public string Patch => "Player/Settings";
        public object InstanceObject => this;
        public void CreateAsset()
        {
            
        }

        [SerializeField, FoldoutGroup("Move Settings")]
        private float _speedRotateRaft;

        [SerializeField, FoldoutGroup("Move Settings")]
        private float _speedRotateWater;

        [SerializeField, FoldoutGroup("Move Settings")]
        private float _speedStopAnimation;

        [SerializeField, FoldoutGroup("Move Settings")]
        private float _maxHeightMoveWater;
        
        [SerializeField, FoldoutGroup("Move Settings")]
        private Vector2 _minMaxHorizontal;

        [SerializeField, FoldoutGroup("Move Settings")]
        private float _speedMoveLadder;
        
        [SerializeField, FoldoutGroup("Jump Settings")]
        private AnimationCurve _curveJump;

        [SerializeField, FoldoutGroup("Jump Settings")]
        private float _heightJump;

        [SerializeField, FoldoutGroup("Jump Settings")]
        private float _speedJump;

        [SerializeField, FoldoutGroup("Jump Settings")]
        private float _distanceJump;

        [SerializeField, FoldoutGroup("Jump To Raft")]
        private AnimationCurve _curveJumpRaft;

        [SerializeField, FoldoutGroup("Jump To Raft")]
        private float _speedJumpToRaft;

        [SerializeField, FoldoutGroup("Jump To Raft")]
        private float _heightJumpToRaft;
        
        public float SpeedRotateRaft => _speedRotateRaft;

        public float SpeedRotateWater => _speedRotateWater;

        public AnimationCurve CurveJump => _curveJump;

        public float HeightJump => _heightJump;

        public float SpeedJump => _speedJump;

        public float DistanceJump => _distanceJump;

        public float SpeedStopAnimation => _speedStopAnimation;

        public float MaxHeightMoveWater => _maxHeightMoveWater;

        public AnimationCurve CurveJumpRaft => _curveJumpRaft;

        public float SpeedJumpToRaft => _speedJumpToRaft;

        public float HeightJumpToRaft => _heightJumpToRaft;

        public Vector2 MinMaxHorizontal => _minMaxHorizontal;

        public float SpeedMoveLadder => _speedMoveLadder;
    }
}

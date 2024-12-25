using Game.Scripts.Core;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Pool;
using UnityEngine;

namespace Game.Scripts.Player.PlayerController.PlayerStates
{
    public class PlayerJumpToWater : EntityState
    {
        public EntityPlayer Player => (EntityPlayer)Entity;
        private int _hasJump = Animator.StringToHash("IsGround");

        private bool _isRippleSpawned;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private Vector3 _splashPosition;
        private PoolObjects<ParticleController> _pool;
        private float _progress;

        public PlayerJumpToWater(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
        {
            var parent = new GameObject("SplashEffectPool").transform;
            //TODO: Вынести в отдельный пул
            _pool = new PoolObjects<ParticleController>(Player.EffectSplashWater, parent.transform, 5);
        }

        public override void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            var position = Vector3.Lerp(_startPosition, _endPosition, _progress);
            position.y += Player.Settings.CurveJump.Evaluate(_progress) * Player.Settings.HeightJump;
            _progress += deltaTime * Player.Settings.SpeedJump;
            Player.Rb.MovePosition(position);
            
            if (_progress >= 1f)
            {
                stateMachine.SetState<PlayerWaterState>();
            }

            if (_progress >= 0.8f)
            {
                if(_isRippleSpawned) return;
                _isRippleSpawned = true;
                
                var bubble = _pool.GetFree();
                bubble.transform.position = _splashPosition;
                bubble.transform.rotation = Quaternion.Euler(-90, 0, 0);
                bubble.gameObject.SetActive(true);
            }
        }

        public override void Enter()
        {
            _isRippleSpawned = false;
            Player.Rb.useGravity = false;
            Player.Animator.SetBool(_hasJump, false);
            _startPosition = Player.transform.position;
            _startPosition.z = 0f;
            _endPosition = _startPosition + (Player.Components.ModelObject.forward * Player.Settings.DistanceJump);
            _splashPosition = _endPosition;
            _splashPosition.y = 0;
            _endPosition.y = -1.5f;
            _endPosition.z = 0f;
            _progress = 0f;
        }
    }
}
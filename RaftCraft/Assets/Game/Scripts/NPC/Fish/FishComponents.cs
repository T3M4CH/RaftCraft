using Game.Scripts.DamageEffector;
using Game.Scripts.NPC.Fish.Systems;
using Game.Scripts.Player.WeaponController.AnimationAttack;
using Game.Scripts.Triggers;
using Pathfinding;
using UnityEngine;

namespace Game.Scripts.NPC.Fish
{
    [System.Serializable]
    public struct FishComponents 
    {
        [field: SerializeField] public Seeker Seeker { get; private set; }
        [field: SerializeField] public DamageFeedback FeedbackDamage { get; private set; }
        [field: SerializeField] public CollisionEvent EventTarget { get; private set; }
        [field: SerializeField] public Animator[] Animator { get; private set; }
        [field: SerializeField] public Renderer[] Renderers { get; private set; }
        [field: SerializeField] public DamageFeedback DamageFeedback { get; private set; }
        [field: SerializeField] public AnimationAttackComponent[] AttackComponents { get; private set; }

        [field: SerializeField] public Rigidbody Rb { get; private set; }
        
        [field: SerializeField] public FishBar Bar { get; private set; }
        
        [field: SerializeField] public ObjectMoveToTarget MoveToTarget { get; private set; }
    }
}

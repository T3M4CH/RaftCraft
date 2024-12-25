using System;
using System.Collections.Generic;
using Game.Scripts.Player.EntityGame;
using Sirenix.OdinInspector;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.Scripts.Player.Collision
{
    [BurstCompile()]
    public class EntityCollisionRayCast : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Settings")] private float _offset;
        [SerializeField, FoldoutGroup("Settings")] private Vector3 _offsetPosition;
        [SerializeField, FoldoutGroup("Settings")] private Entity _entity;
        [SerializeField, FoldoutGroup("Settings")] private float _distanceRay = 2f;
        [SerializeField, FoldoutGroup("Settings")] private LayerMask _mask;

        private HashSet<IUpdateRayCast> _listeners = new HashSet<IUpdateRayCast>();

        private Transform _playerPosition;


        private void Start()
        {
            _playerPosition = new GameObject("TargetPlayer").transform;
        }

        [BurstCompile(FloatPrecision = FloatPrecision.Low, OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
        private void Update()
        {
            _playerPosition.position = transform.position;
            _playerPosition.rotation = Quaternion.Euler(0f, _entity.Components.ModelObject.localEulerAngles.y, 0f);
            var result = new NativeArray<RaycastHit>(2, Allocator.Persistent);
            var commands = new NativeArray<RaycastCommand>(2, Allocator.Persistent);
            var origin = (transform.position + (_playerPosition.forward * _offset)) + _offsetPosition;
            var direction = Vector3.down * _distanceRay;
            Debug.DrawRay(origin, direction, Color.green);
            Debug.DrawRay(origin + _playerPosition.forward * _offset, direction, Color.red);
            commands[0] = new RaycastCommand(origin, direction.normalized, new QueryParameters(_mask), _distanceRay);
            commands[1] = new RaycastCommand(origin + _playerPosition.forward * _offset, direction.normalized, new QueryParameters(_mask), _distanceRay);
            JobHandle handle = RaycastCommand.ScheduleBatch(commands, result, 1, 1, default(JobHandle));
            handle.Complete();
            
            if (result[0].collider != null && result[1].collider != null)
            {
                if (result[0].collider == result[1].collider)
                {
                    var data = new CastData()
                    {
                        target = result[0].collider,
                        point = result[0].point
                    };

                    if (Mathf.Abs(data.point.z) <= 0.1f)
                    {
                        foreach (var listener in _listeners)
                        {
                            if (listener.HaveListener == false)
                            {
                                continue;
                            }

                            listener.UpdateCast(data);
                        }
                    }
                }
            }

            result.Dispose();
            commands.Dispose();
        }

        public void AddListener(IUpdateRayCast listener)
        {
            if (_listeners.Contains(listener))
            {
                return;
            }
            _listeners.Add(listener);
        }
    }
    
    public struct CastData
    {
        public Collider target;
        public Vector3 point;
    }
}

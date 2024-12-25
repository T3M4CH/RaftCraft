using System;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.CameraSystem
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class BaseVirtualCamera : MonoBehaviour
    {
        [Button]
        public void Initialize()
        {
            Camera = GetComponent<CinemachineVirtualCamera>();
        }

        public BaseVirtualCamera SetFollowAt(Transform target)
        {
            Camera.Follow = target;

            return this;
        }

        public BaseVirtualCamera SetLookAt(Transform target)
        {
            Camera.LookAt = target;

            return this;
        }

        [field: SerializeField] public CinemachineVirtualCamera Camera { get; private set; }
    }
}
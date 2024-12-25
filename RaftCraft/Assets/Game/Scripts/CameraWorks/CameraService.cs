using Game.Scripts.CameraSystem.Interfaces;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Utils;
using System.Threading;
using System.Linq;
using UnityEngine;
using System;

using Object = UnityEngine.Object;

namespace Game.Scripts.CameraSystem
{
    public class CameraService : ICameraService
    {
        private Camera _camera;
        private BaseVirtualCamera _activeCamera;
        private CancellationTokenSource _animationCancellation;

        private readonly BaseVirtualCamera[] _cameras;
        
        public float FOV { get; private set; }
        
        public CameraService(SerializableCamerasSettings camerasSettings)
        {
            var cameras = camerasSettings.Cameras;
            
            var parent = new GameObject("CameraController").transform;
            Object.DontDestroyOnLoad(parent);
            
            var mainCamera = Object.Instantiate(camerasSettings.MainCameraBrain, parent);
            _cameras = Enumerable.Range(0, cameras.Length).Select(id => Object.Instantiate(cameras[id], parent)).ToArray();
            
            _activeCamera = _cameras[0];
            _camera = mainCamera.GetComponent<Camera>();
        }

        public void SetLookAndFollowToAll(Transform target)
        {
            foreach (var baseVirtualCamera in _cameras)
            {
                baseVirtualCamera.SetLookAt(target).SetFollowAt(target);
            }
        }
        
        public BaseVirtualCamera GetActiveCamera()
        {
            return _activeCamera;
        }

        public void SetCanvas(Canvas canvas)
        {
            canvas.worldCamera = _camera;
            canvas.planeDistance = 0;
        }

        public void SetFovInstantly(float value)
        {
            _animationCancellation?.Cancel();
            _animationCancellation?.Dispose();
            _animationCancellation = null;

            _activeCamera.Camera.m_Lens.FieldOfView = value;

            FOV = value;
        }

        public void SetFov(float value, float duration)
        {
            _animationCancellation?.Cancel();
            _animationCancellation?.Dispose();
            _animationCancellation = null;

            _animationCancellation = new CancellationTokenSource();
            var token = _animationCancellation.Token;

            PerformChangingFov(value, duration, token);
        }


        private async void PerformChangingFov(float value, float duration, CancellationToken token)
        {
            var delay = duration;
            var constDelay = delay;
            var initialFov = FOV;
            while (delay > 0)
            {
                await UniTask.Yield(PlayerLoopTiming.LastUpdate);

                if (token.IsCancellationRequested)
                {
                    break;
                }

                delay -= Time.deltaTime;

                var newFov = Mathf.Lerp(value, initialFov, delay / constDelay);

                _activeCamera.Camera.m_Lens.FieldOfView = newFov;

                FOV = newFov;
            }
        }

        public BaseVirtualCamera ChangeActiveCamera(int id)
        {
            _activeCamera.Camera.Priority = 0;
            _activeCamera = _cameras[id];
            _activeCamera.Camera.Priority = 1;

            FOV = _activeCamera.Camera.m_Lens.FieldOfView;

            return _activeCamera;
        }

        public T GetCamera<T>() where T : BaseVirtualCamera
        {
            var camera = _cameras.First(camera => camera is T);

            if (camera == null)
            {
                throw new Exception("This camera wasn't presented in collection");
            }

            return camera as T;
        }

        public T ChangeActiveCamera<T>() where T : BaseVirtualCamera
        {
            var camera = _cameras.First(camera => camera is T);

            if (camera == null)
            {
                throw new Exception("This camera wasn't presented in collection");
            }

            _activeCamera.Camera.Priority = 0;
            _activeCamera = camera;
            _activeCamera.Camera.Priority = 1;

            FOV = _activeCamera.Camera.m_Lens.FieldOfView;

            return camera as T;
        }
    }
}
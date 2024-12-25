using UnityEngine;

namespace Game.Scripts.CameraSystem.Interfaces
{
    public interface ICameraService
    {
        void SetCanvas(Canvas canvas);
        void SetFovInstantly(float value);
        void SetFov(float value, float duration);
        void SetLookAndFollowToAll(Transform target);
        BaseVirtualCamera GetActiveCamera();
        T GetCamera<T>() where T : BaseVirtualCamera;
        T ChangeActiveCamera<T>() where T : BaseVirtualCamera;
        BaseVirtualCamera ChangeActiveCamera(int id);

    }
}
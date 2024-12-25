using Game.Scripts.CameraSystem;
using Game.Scripts.CameraSystem.Interfaces;
using Reflex.Core;
using UnityEngine;

public class CameraInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private SerializableCamerasSettings _camerasSettings;
    
    public void InstallBindings(ContainerDescriptor descriptor)
    {
        descriptor.AddSingleton(typeof(CameraService), typeof(ICameraService));
    }
}

using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.CameraSystem;
using Game.Scripts.CameraSystem.Interfaces;
using Game.Scripts.Core.Interface;
using Game.Scripts.DamageEffector.Interface;
using Game.Scripts.Days;
using Game.Scripts.GameSettings;
using Game.Scripts.Joystick.Extras;
using Game.Scripts.DamageEffector;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.Player.WeaponController.Interface;
using Game.Scripts.Player.WeaponController.WeaponInventory;
using Game.Scripts.Player.WeaponController.WeaponsData;
using Game.Scripts.Player.WeaponController.WeaponShop;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using Game.Scripts.Sound;
using Game.Scripts.TimeController;
using Reflex.Core;
using UnityEngine;

namespace Game.Scripts.ReflexInstallers
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        [field: SerializeField] public CellGameSettings PrefabCellSettings { get; private set; }
        [field: SerializeField] public GameSave SaveManager { get; private set; }
        [SerializeField] private SerializableSoundManagerSettings _soundManagerSettings;
        [SerializeField] private SerializableCamerasSettings camerasSettings;
        [SerializeField] private GameSettingComponents _settingComponents;
        [SerializeField] private List<WeaponsDataSettings> _weapons = new List<WeaponsDataSettings>();
        [SerializeField] private WeaponShopData _weaponShopData;
        [SerializeField] private TimeControllerManager _prefabManager;
        
        
        public void InstallBindings(ContainerDescriptor descriptor)
        {
            print("PROJECT INSTALLER!");
            Application.targetFrameRate = 60;
            var save = Instantiate(SaveManager);
            DontDestroyOnLoad(save);
            
            descriptor.AddInstance(save);
            descriptor.AddInstance(camerasSettings);
            descriptor.AddInstance(_settingComponents);
            descriptor.AddInstance(PrefabCellSettings);
            descriptor.AddInstance(_soundManagerSettings);
            descriptor.AddInstance(_weapons);
            descriptor.AddInstance(_weaponShopData);
            descriptor.AddInstance(Instance(), typeof(ITimeController));

            descriptor.AddSingleton(typeof(WeaponInventory), typeof(IInventoryWeapon), typeof(ISelectorWeapon), typeof(IGameObservable<List<WeaponItem>>), typeof(IGameObservable<WeaponItem>));
            descriptor.AddSingleton(typeof(WeaponUpgradeService), typeof(IGameObservable<WeaponUpgrade>), typeof(IWeaponUpgradeService));
            descriptor.AddSingleton(typeof(CameraService),typeof(ICameraService));
            descriptor.AddSingleton(typeof(GameEffectSpawner), typeof(IStartable), typeof(ISpawnEffectService));
            descriptor.AddSingleton(typeof(DayController), typeof(IDayService), typeof(IStartableElement));
            descriptor.AddSingleton(typeof(ResourceService), typeof(IResourceService), typeof(IGameObservable<ResourceItem>));
            descriptor.AddSingleton(typeof(BiomesSceneManager));
            descriptor.AddSingleton(typeof(InputSingleton));
            descriptor.AddSingleton(typeof(WeaponShopController), typeof(IShopView), typeof(IWeaponShop), typeof(IGameObservable<WeaponPrice>));
            
            DOTween.SetTweensCapacity(500, 125);
        }

        private TimeControllerManager Instance()
        {
            var obj = Instantiate(_prefabManager);
            DontDestroyOnLoad(obj.gameObject);
            return obj;
        }
    }
}
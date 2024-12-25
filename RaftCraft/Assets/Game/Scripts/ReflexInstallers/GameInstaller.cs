using Game.Scripts.BattleMode;
using Game.Scripts.Core.Interface;
using Game.Scripts.DamageEffector;
using Game.Scripts.DamageEffector.Data;
using Game.Scripts.GameIndicator;
using Game.Scripts.GameSettings;
using Game.Scripts.Health;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Quest;
using Game.Scripts.Quest.Interfaces;
using Game.Scripts.Raft;
using Game.Scripts.Raft.Interface;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Sound.Interfaces;
using Game.Scripts.Tutorial;
using Game.Scripts.Tutorial.TutorialBuildVendor;
using Game.Scripts.UI.WindowManager;
using Reflex.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.ReflexInstallers
{
    public class GameInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private SerializableDamageEffectSettings _damageEffectSettings;
        [SerializeField] private ResourceServiceSettings _settingsResourceService;
        [SerializeField] private IndicatorData _indicatorData;
        [SerializeField] private ResourcesPrefabs _resourcesPrefabs;

        //TODO: Сделать не monobehom
        [SerializeField] private WindowManager _windowManager;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private BattleModeController _battleModeController;
        [SerializeField] private EffectTextDamageData _prefabTextDamage;
        [SerializeField] private CheckReadyBattle _readyBattle;
        [SerializeField] private InteractionSystem _interactionSystem;
        [SerializeField] private TutorialBuildController _tutorialBuildController;
        [SerializeField] private GameTutorialWood _tutorialWood;
        [SerializeField] private TutorialFishingTwo _fishingTwo;
        
        [SerializeField, FoldoutGroup("Raft")] private RaftConstructor _raftConstructor;

        public void InstallBindings(ContainerDescriptor descriptor)
        {
            descriptor.AddInstance(_damageEffectSettings);
            descriptor.AddInstance(_resourcesPrefabs);
            descriptor.AddInstance(_playerController, typeof(IPlayerService), typeof(IReadyClient));
            descriptor.AddInstance(_windowManager);
            descriptor.AddInstance(_prefabTextDamage);
            descriptor.AddInstance(_settingsResourceService);
            descriptor.AddInstance(_battleModeController, typeof(IBattleService), typeof(IReadyClient));
            descriptor.AddInstance(_raftConstructor, typeof(IRaftStructures));
            descriptor.AddInstance(_indicatorData);
            descriptor.AddInstance(_tutorialBuildController, typeof(IReadyClient));
            descriptor.AddInstance(_tutorialWood, typeof(IReadyClient));
            descriptor.AddInstance(_readyBattle, typeof(IReadyService));
            descriptor.AddInstance(_fishingTwo, typeof(IReadyClient));
            
            descriptor.AddInstance(_interactionSystem);
            descriptor.AddSingleton(typeof(GameIndicatorController), typeof(IControllerIndicator));
            descriptor.AddSingleton(typeof(DamageEffectSpawner));
            descriptor.AddSingleton(typeof(GameSettingService), typeof(IUpdateGameSettings));
            descriptor.AddSingleton(typeof(SoundManager), typeof(IMusicService), typeof(IStartableElement));
            descriptor.AddSingleton(typeof(ResourceSpawner), typeof(IResourceSpawner), typeof(IStartableElement));
            descriptor.AddSingleton(typeof(GameResourceService), typeof(IGameResourceService));
            descriptor.AddSingleton(typeof(QuestManager), typeof(IQuestService), typeof(IReadyClient));
        }
    }
}
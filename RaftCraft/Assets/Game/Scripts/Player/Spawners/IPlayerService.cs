using System;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.ResourceController.LocalPlayerResources;

namespace Game.Scripts.Player.Spawners
{
    public interface IPlayerService
    {
        public void AddListener(Action<EPlayerStates, EntityPlayer> actionSpawn);

        public void RemoveListener(Action<EPlayerStates, EntityPlayer> actionSpawn);
        
        public PlayerResources PlayerResources { get; }
        public IPlayerUpgradeSettings UpgradeSettings { get; }
        public IPlayerUpgradeService UpgradeService { get; }
    }
}

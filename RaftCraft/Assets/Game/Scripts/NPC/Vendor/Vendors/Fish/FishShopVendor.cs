using Game.Scripts.BattleMode;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowManager.Windows;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Prefabs.NPC.Vendors
{
    public class FishShopVendor : BaseVendor
    {
        [field: SerializeField] public GameObject ClaimButton { get; private set; }
        [Inject]
        private void Construct(IBattleService battleService)
        {
            Initialize(battleService);
        }

        protected override void ShowWindow(WindowManager windowManager)
        {
            windowManager.OpenWindow<WindowShopFish>();
        }
    }
}

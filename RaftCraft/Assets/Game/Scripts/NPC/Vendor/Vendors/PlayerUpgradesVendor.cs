using System;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowVendors;
using Game.Scripts.UI.WindowVendors.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Prefabs.NPC.Vendors
{
    public class PlayerUpgradesVendor : MonoBehaviour, IUiInteraction
    {
        private WindowManager _windowManager;

        [SerializeField, FoldoutGroup("Settings")] private EVendorType _typeVendor;

        public event Action OnEnterInteraction = () => { }; 
        public event Action OnExitInteraction = () => { };
        public bool IsAbleEverywhere => false;
        public bool Interaction => true;
        public InteractionType CurrentTypeInteraction => InteractionType.Npc;
        public float DelayAction => 0f;
        
        public void Action()
        {
        }
        

        public void EnterInteraction()
        {
            OnEnterInteraction.Invoke();
        }

        public void ExitInteraction()
        {
            OnExitInteraction.Invoke();
            if (_windowManager != null)
            {
                //TODO: Weapon selector??
                _windowManager.GetWindow<SelectorWindow>().SetVendorType(EVendorType.None);
            }
        }

        public virtual void OpenWindow(WindowManager windowManager)
        {
            _windowManager = windowManager;
            _windowManager.OpenWindow<SelectorWindow>().SetVendorType(_typeVendor);
        }
    }
}

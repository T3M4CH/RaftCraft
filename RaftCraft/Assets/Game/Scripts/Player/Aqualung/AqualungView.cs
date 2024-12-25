using System;
using System.Collections.Generic;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Game.Scripts.Player.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Player.Aqualung
{
    public class AqualungView : MonoBehaviour
    {
        [System.Serializable]
        public class AqualungLinks
        {
            [field: SerializeField] public int LevelMin { get; private set; }
            [field: SerializeField] public int LevelMax { get; private set; }
            [field: SerializeField] public List<GameObject> AqualungObjects { get; private set; }

            public void SetState(bool state)
            {
                foreach (var link in AqualungObjects)
                {
                    link.SetActive(state);
                }
            }
        }
        
        [SerializeField, FoldoutGroup("Settings")] private EntityPlayer _entity;
        [SerializeField, FoldoutGroup("Settings")] private List<AqualungLinks> _links = new List<AqualungLinks>();
        private void Start()
        {
            if (_entity == null)
            {
                return;
            }
            
            _entity.StateMachine.OnEnterState += StateMachineOnOnEnterState;
            StateMachineOnOnEnterState(_entity.StateMachine.CurrentEntityState, _entity);
        }

        private void StateMachineOnOnEnterState(EntityState state, Entity entity)
        {
            switch (state)
            {
                case PlayerPlotState:
                    HideAqualung();
                    ViewAqualung(0);
                    break;
                case PlayerJumpToWater:
                    HideAqualung();
                    ViewAqualung(_entity.PlayerSettings.GetLevel(EPlayerUpgradeType.Oxygen) - 1);
                    break;
                case PlayerFallWater:
                    HideAqualung();
                    ViewAqualung(_entity.PlayerSettings.GetLevel(EPlayerUpgradeType.Oxygen) - 1);
                    break;
            }
        }

        private void ViewAqualung(int level)
        {
            var link = GetLink(level);
            if (link == null)
            {
                link = _links[^1];
            }
            
            link.SetState(true);
        }

        private void HideAqualung()
        {
            foreach (var link in _links)
            {
                link.SetState(false);
            }
        }

        private AqualungLinks GetLink(int level)
        {
            foreach (var link in _links)
            {
                if (link.LevelMin >= level && link.LevelMax > level)
                {
                    return link;
                }
            }

            return null;
        }
        
        private void OnDisable()
        {
            if (_entity == null)
            {
                return;
            }
            
            _entity.StateMachine.OnEnterState -= StateMachineOnOnEnterState;
        }
    }
}

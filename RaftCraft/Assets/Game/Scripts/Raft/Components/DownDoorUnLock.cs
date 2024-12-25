using System;
using System.Collections.Generic;
using Game.Scripts.Days;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Scripts.Raft.Components
{
    public class DownDoorUnLock : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _disabled;
        [SerializeField] private List<GameObject> _enabled;
        [SerializeField] private int _dayUnLock;
        
        private IDayService _dayService;
        
        [Inject]
        private void Construct(IDayService dayService)
        {
            _dayService = dayService;
        }

        private void Start()
        {
            _dayService.OnDayStart += DayServiceOnOnDayStart;
        }

        private void DayServiceOnOnDayStart(int day)
        {
            if (day >= _dayUnLock)
            {
                foreach (var disable in _disabled)
                {
                    disable.SetActive(false);
                }

                foreach (var enable in _enabled)
                {
                    enable.SetActive(true);
                }
            }
        }

        private void OnDestroy()
        {
            _dayService.OnDayStart -= DayServiceOnOnDayStart;
        }
    }
}

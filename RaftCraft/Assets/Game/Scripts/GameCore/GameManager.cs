using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Days;
using Game.Scripts.Player.Spawners;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Scripts.GameCore
{
    public class GameManager : MonoBehaviour, IGameStateChangeService
    {
        public enum GameState
        {
            Load,
            StartDay,
            Gameplay
        }

        private IDayService _dayService;
        private IPlayerService _service;
        private IEnumerable<IStartableElement> _startableElements;

        [Inject]
        private void Construct(IEnumerable<IStartableElement> startableElements)
        {
            _startableElements = startableElements;
        }

        private void Start()
        {
            foreach (var startableElement in _startableElements.OrderBy(element => element.Priority))
            {
                startableElement.Execute();
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Days;
using Game.Scripts.Quest.Interfaces;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;

namespace Game.Scripts.Quest
{
    public enum ReadyState
    {
        Disabled,
        Timer,
        Enabled
    }
    
    public class CheckReadyBattle : MonoBehaviour, IReadyService
    {
        public event Action<float> OnChangeTime; 
        private List<IReadyClient> _clients;
        private Action<ReadyState> OnChangeState;
        private float _delay;
        private IDayService _dayService;

        private ReadyState _state;

        private ReadyState State
        {
            get => _state;
            set
            {
                _state = value;
                Debug.Log($"Ready state: {_state}");
                OnChangeState?.Invoke(_state);
                switch (_state)
                {
                    case ReadyState.Disabled:
                        break;
                    case ReadyState.Timer:
                        break;
                    case ReadyState.Enabled:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        [Inject]
        private void Construct(IEnumerable<IReadyClient> clients, IDayService dayService)
        {
            _clients = clients.ToList();
            _dayService = dayService;
            foreach (var client in _clients)
            {
                client.OnChangeReady += OnChangeReady;
            }
        }

        private void Start()
        {
            State = HaveAllReady() ? ReadyState.Enabled : ReadyState.Disabled;
        }

        private void OnChangeReady(IReadyClient client)
        {
            if (client.Delay != 0f && client.IsReady && _dayService.CurrentDay > 2)
            {
                StartCoroutine(WaitReady(client.Delay));
            }
            else
            {
                if (State != ReadyState.Timer)
                {
                    State = HaveAllReady() ? ReadyState.Enabled : ReadyState.Disabled;
                }
            }
        }

        private IEnumerator WaitReady(float delay)
        {
            State = ReadyState.Timer;
            var time = delay;
            while (time > 0f)
            {
                time -= Time.smoothDeltaTime;
                OnChangeTime?.Invoke(time);
                yield return null;
            }

            State = HaveAllReady() ? ReadyState.Enabled : ReadyState.Disabled;
        }

        private bool HaveAllReady()
        {
            return _clients.All(client => client.IsReady);
        }

        public void AddListener(Action<ReadyState> action)
        {
            OnChangeState += action;
            action?.Invoke(State);
        }

        public void RemoveListener(Action<ReadyState> action)
        {
            OnChangeState -= action;
        }

        public void AddClient(IReadyClient client)
        {
            if (_clients.Contains(client) == false)
            {
                client.OnChangeReady += OnChangeReady;
                _clients.Add(client);
            }
        }

        public void RemoveClient(IReadyClient client)
        {
            client.OnChangeReady -= OnChangeReady;
            _clients.Remove(client);
        }
        
        private void OnDestroy()
        {
            foreach (var client in _clients)
            {
                client.OnChangeReady -= OnChangeReady;
            }
        }
    }
}
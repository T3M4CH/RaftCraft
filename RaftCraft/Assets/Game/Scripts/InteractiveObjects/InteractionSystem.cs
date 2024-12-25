using System;
using System.Collections;
using System.Collections.Generic;
using Game.Prefabs.NPC.Vendors;
using Game.Scripts.BattleMode;
using Game.Scripts.Core.Interface;
using Game.Scripts.InteractiveObjects.Interfaces;
using Game.Scripts.NPC;
using Game.Scripts.Player;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Quest.Interfaces;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowManager.Windows;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.InteractiveObjects
{
    public class InteractionSystem : MonoBehaviour, IReadyClient
    {
        private EntityPlayer _currentPlayer;
        private IPlayerService _spawner;

        private WaitForSeconds _delayInteraction;
        private WindowManager _windowManager;

        private IBattleService _battleService;

        private bool _interaction;
        private WaitForSeconds _forSeconds = new(0f);

        private Dictionary<IInteraction, Coroutine> _poolInteraction;
        private MovementState _stateMove;
        private void Awake()
        {
            _poolInteraction = new Dictionary<IInteraction, Coroutine>();
        }

        [Inject]
        private void Construct(IPlayerService spawner, WindowManager windowManager, IBattleService battleService)
        {
            _windowManager = windowManager;
            _spawner = spawner;
            _battleService = battleService;
            _spawner.AddListener(SpawnerOnOnSpawnPlayer);
        }
        
        private void Start()
        {
            _battleService.OnChangeState += OnBattleChange;
        }

        private void SpawnerOnOnSpawnPlayer(EPlayerStates state, EntityPlayer player)
        {
            if (state != EPlayerStates.SpawnPlayer) return;
            if (_currentPlayer != null)
            {
                _currentPlayer.StateMachine.GetState<PlayerPlotState>().OnChangeMovement -= OnOnChangeMovement;
            }

            _currentPlayer = player;
            _currentPlayer.StateMachine.GetState<PlayerPlotState>().OnChangeMovement += OnOnChangeMovement;
            _currentPlayer.StateMachine.OnEnterState += StateMachineOnOnEnterState;
            _currentPlayer.Components.Collision.OnEventTriggerEnter += CollisionOnEventOnEventTriggerEnter;
            _currentPlayer.Components.Collision.OnEventTriggerExit += CollisionOnOnEventTriggerExit;
        }

        private void OnOnChangeMovement(MovementState stateMove)
        {
            _stateMove = stateMove;
        }

        private void CollisionOnEventOnEventTriggerEnter(Collider collider)
        {
            if (_interaction == false)
            {
                return;
            }

            if (collider.TryGetComponent<IInteraction>(out var interaction) == false)
            {
                return;
            }

            if (_currentPlayer.StateMachine.GetState<PlayerPlotState>().IsActive == false)
            {
                return;
            }

            if (interaction.Interaction)
            {
                StartLogicInteraction(interaction);
            }
        }

        private void CollisionOnOnEventTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent<IInteraction>(out var interaction) == false)
            {
                return;
            }

            if (_poolInteraction == null)
            {
                _poolInteraction = new Dictionary<IInteraction, Coroutine>();
            }

            if (_poolInteraction.TryGetValue(interaction, out var value))
            {
                if (value != null)
                {
                    StopCoroutine(value);
                }
                interaction.ExitInteraction();
                Debug.Log($"Remove interaction");
                RemoveInteraction(interaction);
            }
        }

        private void StartLogicInteraction(IInteraction interaction)
        {
            if (_interaction == false)
            {
                return;
            }
            
            AddInteraction(interaction);
        }

        private IEnumerator CoroutineInteraction(IInteraction interaction, float delay)
        {
            interaction.EnterInteraction();
            while (true)
            {
                if (interaction.Interaction == false)
                {
                    yield return null;
                    continue;
                }
                if (HaveMovePlayer() && !interaction.IsAbleEverywhere)
                {
                    yield return null;
                    continue;
                }

                if (HaveRaftPlayer() == false)
                {
                    RemoveInteraction(interaction);
                    yield break;
                }

                yield return new WaitForSeconds(interaction.DelayAction);
                interaction.Action();
                if (interaction.Interaction == false)
                {
                    RemoveInteraction(interaction);
                    yield break;
                }
            }
        }

        private void AddInteraction(IInteraction interaction)
        {
            Debug.Log($"Add interaction");
            _poolInteraction.TryAdd(interaction, null);
            switch (interaction)
            {
                case IUiInteraction type:
                    _poolInteraction[interaction] = StartCoroutine(CoroutineInteraction(interaction,interaction.DelayAction));
                    type.OpenWindow(_windowManager);
                    break;
            }
            switch (interaction.CurrentTypeInteraction)
            {
                case InteractionType.Build:
                    _delay = 0.5f;
                    _poolInteraction[interaction] = StartCoroutine(CoroutineInteraction(interaction,interaction.DelayAction));
                    break;
                case InteractionType.Npc:
                    _delay = 0f;
                    break;
                case InteractionType.Repair:
                    _delay = 0f;
                    _poolInteraction[interaction] = StartCoroutine(CoroutineInteraction(interaction,interaction.DelayAction));
                    break;
            }
            
            OnChangeReady?.Invoke(this);
        }

        private void RemoveInteraction(IInteraction interaction)
        {
            if (_poolInteraction.ContainsKey(interaction))
            {
                _poolInteraction.Remove(interaction);
            }
            
            OnChangeReady?.Invoke(this);
        }

        private bool HaveMovePlayer()
        {
            return _stateMove == MovementState.Move;
        }

        private bool HaveRaftPlayer()
        {
            if (_currentPlayer == null)
            {
                return false;
            }

            return _currentPlayer.Space == LocationSpace.Ground;
        }

        private void StateMachineOnOnEnterState(EntityState state, Entity entity)
        {
            switch (state)
            {
                case PlayerDeathInGround:
                    _currentPlayer.Components.Collision.OnEventTriggerEnter -= CollisionOnEventOnEventTriggerEnter;
                    _currentPlayer.StateMachine.OnEnterState -= StateMachineOnOnEnterState;
                    _currentPlayer.Components.Collision.OnEventTriggerExit -= CollisionOnOnEventTriggerExit;
                    break;
                case PlayerDeathInWater:
                    _currentPlayer.Components.Collision.OnEventTriggerEnter -= CollisionOnEventOnEventTriggerEnter;
                    _currentPlayer.StateMachine.OnEnterState -= StateMachineOnOnEnterState;
                    _currentPlayer.Components.Collision.OnEventTriggerExit -= CollisionOnOnEventTriggerExit;
                    break;
            }
        }

        private void OnDestroy()
        {
            _battleService.OnChangeState -= OnBattleChange;

            if (_spawner != null)
            {
                _spawner.RemoveListener(SpawnerOnOnSpawnPlayer);
            }
        }

        private void OnBattleChange(BattleState battleState)
        {
            switch (battleState)
            {
                case BattleState.Idle:
                    _interaction = true;
                    break;
                case BattleState.Fight:
                    foreach (var interaction in _poolInteraction)
                    {
                        if(interaction.Value == null) continue;
                        
                        interaction.Key.ExitInteraction();
                        StopCoroutine(interaction.Value);
                    }
                    
                    _poolInteraction.Clear();
                    OnChangeReady?.Invoke(this);

                    _interaction = false;
                    break;
            }
        }

        public event Action<IReadyClient> OnChangeReady;

        public bool IsReady => _poolInteraction.Count == 0;
        private float _delay;
        public float Delay => _delay;
    }
}
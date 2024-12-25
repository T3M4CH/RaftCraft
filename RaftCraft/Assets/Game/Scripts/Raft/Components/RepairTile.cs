using System;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.InteractiveObjects.Interfaces;
using Game.Scripts.Quest.Interfaces;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Scripts.Raft.Components
{
    public class RepairTile : MonoBehaviour, IInteraction, IReadyClient
    {
        [SerializeField] private TileEntity _target;
        public bool IsAbleEverywhere => false;

        public bool Interaction
        {
            get
            {
                if (_target == null || _target.gameObject.activeSelf == false)
                {
                    return false;
                }

                return _target.HaveInteraction;
            }
        }
        public InteractionType CurrentTypeInteraction => InteractionType.Repair;
        public float DelayAction => 0f;

        private float _progress;

        private float Progress
        {
            get => _progress;
            set
            {
                _progress = Mathf.Clamp(value, 0f, 1f);
                _target.Bar.SetProgress(_progress / 1f);
                if (_progress >= 1f)
                {
                    _isReady = true;
                    OnChangeReady?.Invoke(this);
                    _target.Heal();
                    _target.Bar.HideProgress();
                }
            }
        }

        private IReadyService _readyService;

        [Inject]
        private void Construct(IReadyService readyService)
        {
            _readyService = readyService;
        }

        private void Start()
        {
            _readyService.AddClient(this);
            _isReady = true;
            OnChangeReady?.Invoke(this);
        }

        public void Action()
        {
            if (_target == null)
            {
                return;
            }

            Progress += Time.smoothDeltaTime;
        }

        public void EnterInteraction()
        {
            if (_target == null)
            {
                return;
            }

            _isReady = false;
            OnChangeReady?.Invoke(this);
            Progress = 0f;
            _target.Bar.ShowProgress();
        }

        public void ExitInteraction()
        {
            if (_target == null)
            {
                return;
            }

            _isReady = true;
            OnChangeReady?.Invoke(this);
            Progress = 0f;
            _target.Bar.HideProgress();
        }

        private bool _isReady;
        public event Action<IReadyClient> OnChangeReady;
        public bool IsReady => _isReady;
        public float Delay => 0f;
    }
}

using System;
using Game.Scripts.DamageEffector;
using Game.Scripts.NPC.Fish.FishStates.Passive;
using Game.Scripts.NPC.Interface;
using Game.Scripts.Player.EntityGame;
using GTapSoundManager.SoundManager;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Game.Scripts.NPC.Fish
{
    [RequireComponent(typeof(Seeker))]
    public class BaseFish : Entity, IHeals, IDamagable, IDead<BaseFish>
    {
        public event Action<float> OnUpdateHealsProgress;
        public event Action<BaseFish> OnDead;

        [SerializeField] private SoundAsset _deadSound;
        [field: SerializeField, FoldoutGroup("Fish Components")] public FishComponents ComponentsFish { get; private set; }

        private int _level;
        private float _heals;
        private float _maxHeals;
        private float _progress;
        private bool _isInitialized;

        private FishData _data;

        [field: SerializeField, FoldoutGroup("Preload Data")]
        public bool IsPreloadData { get; private set; }

        [SerializeField, FoldoutGroup("Preload Data"), ShowIf("IsPreloadData")]
        private FishData _dataPreload;

        private DamageEffectSpawner _damageEffector;

        public Vector3 Position => transform.position;
        public event Action<IDamagable, float> OnDamage;

        [Button("Test Damage")]
        public void TakeDamage(float damage, Vector3 target, bool critical = false)
        {
            Heals -= damage;
            OnDamage?.Invoke(this, damage);
            if (ComponentsFish.DamageFeedback != null)
            {
                ComponentsFish.DamageFeedback.TweenExecute();
            }

            if (Heals <= 0f)
            {
                _deadSound.Play(Random.Range(0.95f, 1.05f));
                OnDead?.Invoke(this);
                OnDead = null;
                gameObject.SetActive(false);
            }
        }

        public void Initialize(FishData fishData, FishHabitat habitat, float? yMin = null, float? yMax = null)
        {
            if (yMin.HasValue && yMax.HasValue)
            {
                YMin = yMin;
                YMax = yMax;
            }

            Data = fishData ?? _dataPreload;
            Habitat = habitat;
            Progress = Data.HealsGive;
            ComponentsFish.Bar.SetStateLevelBar(true);
        }
        
        public void Construct(DamageEffectSpawner damageEffector)
        {
            _damageEffector = damageEffector;
        }

        public bool HaveProgress()
        {
            return Progress > 0f;
        }

        public void RemoveProgress(float value)
        {
            IsTargetFish = true;
            Progress -= value;
        }

        public void ResetProgress()
        {
            Progress = Data.HealsGive;
        }

        public void SetBarrierHeight(float yValue)
        {
            YBarrier = yValue;
        }

        private void SetModelIndex(int index)
        {
            for (var i = 0; i < ComponentsFish.Animator.Length; i++)
            {
                ComponentsFish.Animator[i].gameObject.SetActive(i == index);
                if (i == index)
                {
                    ComponentsFish.Animator[i].gameObject.transform.localPosition = new Vector3(Random.Range(-0.4f, 0.4f), 0f, 0f);
                }
            }

            ComponentsFish.Animator[index].gameObject.SetActive(true);
            ComponentsFish.Animator[index].gameObject.transform.localPosition = new Vector3(Random.Range(-0.4f, 0.4f), 0f, 0f);

            ComponentsFish.DamageFeedback.SetRenderer(ComponentsFish.Renderers[index]);
        }

        public float? YMin { get; private set; }
        public float? YMax { get; private set; }
        public float YBarrier { get; private set; }
        public float YSpawnPosition { get; set; }

        private float Heals
        {
            get => _heals;
            set
            {
                _heals = Mathf.Clamp(value, 0f, _maxHeals);
                OnUpdateHealsProgress?.Invoke(_heals / _maxHeals);
            }
        }

        public int Level
        {
            get => _level;
            private set
            {
                _level = Mathf.Clamp(value, 0, ComponentsFish.Animator.Length);
                ComponentsFish.Bar.SetLevel(_level);
                SetModelIndex(_level - 1);
            }
        }

        public float Progress
        {
            get => _progress;
            private set
            {
                _progress = Mathf.Clamp(value, 0f, Data.HealsGive);
                ComponentsFish.Bar.SetProgress(_progress / Data.HealsGive);
            }
        }

        public FishData Data
        {
            get => _data;
            private set
            {
                _data = value;
                if (_data != null)
                {
                    Level = _data.Level;
                    _maxHeals = _data.Heals;
                    Heals = _maxHeals;
                }
            }
        }

        public bool IsTargetFish { get; set; }

        public override bool HaveLife => Progress > 0f;
        public override LocationSpace Space => LocationSpace.Water;

        public EntityType CurrentType => Type;
        public FishHabitat Habitat { get; private set; }
    }
}
using System;
using System.Linq;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using GTapSoundManager.SoundManager;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.CollectingResources
{
    public class CollectionTutorialResource : CollectingResourceObject
    {
        [SerializeField] private EResourceType _resourceTypeTutorial;
        [SerializeField] private int _countTutorial;

        [SerializeField] private SoundAsset _collect;
        [SerializeField] private GameObject canvasObject;
        [SerializeField] private GameObject _modelCoin;
        [SerializeField] private GameObject _modelWood;
        [SerializeField] private GameObject _modelDiamond;

        private event Action OnCollect = () => { };

        [Inject]
        public void Construct(IGameResourceService resourceService)
        {
            _resourceService = resourceService;
            Initialize(_resourceService, Enumerable.Repeat((_resourceTypeTutorial, _countTutorial), 1));
        }

        public void SetActiveArrow(bool value)
        {
            canvasObject.SetActive(value);
        }

        public void InitializeCallback(Action callback)
        {
            OnCollect = callback;
        }

        public override void UpdateType(EResourceType resourceType)
        {
            _modelCoin.SetActive(false);
            _modelWood.SetActive(false);
            _modelDiamond.SetActive(false);
            switch (resourceType)
            {
                case EResourceType.CoinBlue:
                    _modelCoin.SetActive(true);
                    break;
                case EResourceType.Wood:
                    _modelWood.SetActive(true);
                    break;
                case EResourceType.Crystals:
                    _modelDiamond.SetActive(true);
                    break;
            }
        }

        public override void Collect()
        {
            base.Collect();
            if (_resourceService == null)
            {
                return;
            }

            _resourceService.Add(ResourceType, Count);
            _collect.Play();
            OnCollect?.Invoke();
            OnCollect = null;
            Destroy(gameObject);
        }
    }
}
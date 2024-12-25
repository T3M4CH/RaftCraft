using System;
using System.Linq;
using Game.Scripts.ResourceController.Enums;
using GTapSoundManager.SoundManager;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.Scripts.CollectingResources
{
    public class CollectingGameResource : CollectingResourceObject
    {
        [SerializeField, FoldoutGroup("General")] private ResourceTypeObject[] objectsById;
        [SerializeField] private GameObject multipleObject;

        [SerializeField] private SoundAsset _soundAsset;

        public override void UpdateType(EResourceType resourceType)
        {
            multipleObject.SetActive(false);

            if (Resources?.Count() > 1)
            {
                objectsById.ForEach(model => model.gameObject.SetActive(false));

                multipleObject.SetActive(true);
                return;
            }

            foreach (var obj in objectsById)
            {
                obj.gameObject.SetActive(obj.Type == resourceType);
            }
        }

        public override void Collect()
        {
            if (_resourceService == null)
            {
                return;
            }

            if (Resources.Any())
            {
                foreach (var resource in Resources)
                {
                    _resourceService.Add(resource.Item1, resource.Item2);
                }
                
                _soundAsset.Play();
                gameObject.SetActive(false);
            }
            base.Collect();
        }
    }
}
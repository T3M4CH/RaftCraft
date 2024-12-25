using System;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Tutorial.TutorialBuildVendor
{
    public abstract class TutorialBuildVendor : MonoBehaviour
    {
        public Action<TutorialBuildVendor> OnComplete; 

        [SerializeField, ReadOnly] private string _guid;
        [SerializeField, FoldoutGroup("Resource")] protected EResourceType _resourceTarget;

        public abstract int Cost();
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_guid))
            {
                _guid = Guid.NewGuid().ToString();
            }
        }

        public virtual bool ShowArrow()
        {
            return true;
        }

        public abstract void StartTutorial();

        public bool HaveResource(IGameResourceService resourceService)
        {
            return resourceService.HaveCount(_resourceTarget, Cost());
        }

        public bool HaveComplete(GameSave gameSave)
        {
            return gameSave.GetData($"Tutorial_{_guid}", false);
        }

        public virtual void Complete(GameSave gameSave)
        {
            gameSave.SetData($"Tutorial_{_guid}", true);
        }

        public virtual Transform Target()
        {
            return transform;
        }
}
}

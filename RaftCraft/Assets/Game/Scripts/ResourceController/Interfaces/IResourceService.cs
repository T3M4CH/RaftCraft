using System;
using Game.Scripts.ResourceController.Enums;

namespace Game.Scripts.ResourceController.Interfaces
{
    public interface IResourceService
    {
        public event Action<EResourceType, int, TypeAddResource> OnEventAddResource; 

        int GetValue(EResourceType type);
        int GetValueLocal(EResourceType type);
        void Add(EResourceType type,int value);
        void AddLocal(EResourceType type, int value);
        bool TryRemove(EResourceType type, int value);
        bool TryRemoveLocal(EResourceType type, int value);
        bool HaveCount(EResourceType type, int value);
        void Save();
    }
}
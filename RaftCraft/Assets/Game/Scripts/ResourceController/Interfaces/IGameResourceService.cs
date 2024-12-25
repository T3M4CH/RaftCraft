using System.Collections.Generic;
using Game.Scripts.ResourceController.Enums;

namespace Game.Scripts.ResourceController.Interfaces
{
    public interface IGameResourceService
    {
        public void Add(EResourceType type, int count);
        public bool TryRemove(EResourceType type, int count);
        public bool HaveCount(EResourceType type, int count);
        public IEnumerable<(EResourceType, int)> LocalItems { get; }
    }
}
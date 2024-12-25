using Game.Scripts.ResourceController.Enums;

namespace Game.Scripts.ResourceController
{
    [System.Serializable]
    public class ResourceItem
    {
        public EResourceType Type;
        public int Count;
        public int TempCount;

        public bool HaveGlobal { get; }
        public int Index;
        
        public ResourceItem(bool haveGlobal)
        {
            HaveGlobal = haveGlobal;
        }
    }
}
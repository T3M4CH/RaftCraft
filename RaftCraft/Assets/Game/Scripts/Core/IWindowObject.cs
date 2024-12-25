namespace Game.Scripts.Core
{
    public interface IWindowObject
    {
        public string Patch { get; }

        public object InstanceObject() => this;
        
        public void CreateAsset();
    }
}

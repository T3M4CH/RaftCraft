namespace Game.Scripts.Core.Interface
{
    public interface IGameObserver<T>
    {
        public void PerformNotify(T data);
    }
}

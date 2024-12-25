namespace Game.Scripts.Core.Interface
{
    public interface IGameObservable<T>
    {
        public void AddObserver(IGameObserver<T> gameObserver);
        public void RemoveObserver(IGameObserver<T> gameObserver);
        public void Notify(T data);
    }
}

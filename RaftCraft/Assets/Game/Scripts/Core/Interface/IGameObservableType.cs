using System;
namespace Game.Scripts.Core.Interface
{
    public interface IGameObservableType<T, D> where T : Enum
    {
        public void AddObserver(IGameObserverType<T, D> gameObserver);
        public void RemoveObserver(IGameObserverType<T, D> gameObserver);
        public void Notify(T type, D data);
    }
}
using System;

namespace Game.Scripts.Core.Interface
{
    public interface IGameObserverType<in T, in D> where T : Enum
    {
        public void Update(T type, D data);
    }
}
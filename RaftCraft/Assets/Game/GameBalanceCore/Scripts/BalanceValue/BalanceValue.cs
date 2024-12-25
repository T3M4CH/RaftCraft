using System.Collections.Generic;
using Game.Scripts.Core.Interface;
using UnityEngine.UI;

namespace Game.GameBalanceCore.Scripts.BalanceValue
{
    public abstract class BalanceValue<T> : IGameObservable<T>
    {
        protected readonly T _minValue;
        protected readonly T _maxValue;
        protected readonly string _nameValue;
        private List<IGameObserver<T>> _gameObservers;
        
        private T _value;
        
        public BalanceValue(T min, T max, string nameValue)
        {
            _minValue = min;
            _maxValue = max;
            _nameValue = nameValue;
            _gameObservers = new List<IGameObserver<T>>();
        }


        public virtual void InitSlider(Slider slider)
        {
            
        }
        public abstract void SetValue(T value);
        public abstract T GetValue(T defaultValue);
        public abstract T GetCurrent();
        public void AddObserver(IGameObserver<T> gameObserver)
        {
            _gameObservers.Add(gameObserver);
        }

        public virtual string GetName()
        {
            return _nameValue;
        }

        public void RemoveObserver(IGameObserver<T> gameObserver)
        {
            _gameObservers.Remove(gameObserver);
        }

        public void Notify(T data)
        {
            foreach (var observer in _gameObservers)
            {
                if (observer != null)
                {
                    observer.PerformNotify(data);
                }
            }
        }
    }
}

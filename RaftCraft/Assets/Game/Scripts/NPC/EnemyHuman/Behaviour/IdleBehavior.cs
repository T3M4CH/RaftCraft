using System;
using System.Collections;
using UnityEngine;

namespace Game.Scripts.NPC
{
    public class IdleBehavior : IWait
    {
        private readonly ICoroutineRunner _coroutineRunner;

        private Coroutine _currentWaitCoroutine; 

        public IdleBehavior(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }
        
        public void Wait(Action callback, float time)
        {
            _currentWaitCoroutine = _coroutineRunner.StartCoroutine(WaitCoroutine(callback, time));
        }

        public void Trigger()
        {
            _coroutineRunner.StopCoroutine(_currentWaitCoroutine);
        }

        private IEnumerator WaitCoroutine(Action callback, float time)
        {
            yield return new WaitForSeconds(time);
            
            callback?.Invoke();
        }
    }
}
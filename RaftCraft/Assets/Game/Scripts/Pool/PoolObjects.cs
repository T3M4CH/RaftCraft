using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.Pool
{
    public class PoolObjects<T> : IEnumerable<T>, IDisposable where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly Transform _content;
        private readonly Queue<T> _pool;

        public PoolObjects(T prefab, Transform content, int preloadCount)
        {
            _prefab = prefab;
            _content = content;
            _pool = new Queue<T>();
            for (var i = 0; i < preloadCount; i++)
            {
                CreateObject();
            }
        }

        public T GetFree(bool returnActive = false)
        {
            foreach (var obj in _pool)
            {
                if (obj.gameObject.activeSelf == false)
                {
                    obj.gameObject.SetActive(returnActive);
                    return obj;
                }
            }

            return CreateObject(returnActive);
        }

        public void ClosePoll()
        {
            foreach (var obj in _pool)
            {
                obj.gameObject.SetActive(false);
            }
        }

        private T CreateObject(bool returnActive = false)
        {
            var obj = Object.Instantiate(_prefab, _content);
            obj.gameObject.SetActive(returnActive);
            _pool.Enqueue(obj);
            return obj;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _pool.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            _pool.Clear();
        }
    }
}
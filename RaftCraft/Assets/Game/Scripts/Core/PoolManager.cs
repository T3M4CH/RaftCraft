using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    private Dictionary<string, LinkedList<object>> _pools = new Dictionary<string, LinkedList<object>>();

    #region GetGenerics

        public T GetPool<T>(T prefab, Vector3 pos, Quaternion quaternion) where T : MonoBehaviour
        {
            string name = prefab.gameObject.name;
            if (!_pools.ContainsKey(name))
                _pools[name] = new LinkedList<object>();

            T result;

            if(_pools[name].Count > 0)
            {
                result = GetPoolFirst<T>(name, pos, quaternion);
                return result;
            }
            
            result = Object.Instantiate(prefab, pos, quaternion);
            result.gameObject.name = name;

            return result;
        }

        public T GetPool<T>(T prefab, Vector3 pos) where T : MonoBehaviour
        {
            string name = prefab.gameObject.name;
            if (!_pools.ContainsKey(name))
                _pools[name] = new LinkedList<object>();

            T result;
            var quaternion = prefab.transform.rotation;

            if(_pools[name].Count > 0)
            {
                result = GetPoolFirst<T>(name, pos, quaternion);
                return result;
            }
            
            result = Object.Instantiate(prefab, pos, quaternion);
            result.gameObject.name = name;

            return result;
        }
        
        private T GetPoolFirst<T>(string name, Vector3 pos, Quaternion quaternion) where T : MonoBehaviour
        {
            T result = _pools[name].First.Value is T 
                ? _pools[name].First.Value as T 
                : (_pools[name].First.Value as GameObject).GetComponent<T>();
            
            result.transform.rotation = quaternion;
            result.transform.position = pos;
            result.gameObject.SetActive(true);
            _pools[name].RemoveFirst();

            return result;
        }

    #endregion
    
    #region GetGameObject

        public GameObject GetPool(GameObject prefab, Vector3 pos, Quaternion quaternion)
        {
            string name = prefab.name;
        
            if (!_pools.ContainsKey(name))
                _pools[name] = new LinkedList<object>();

            GameObject result;

            if(_pools[name].Count > 0)
            {
                result = GetPoolFirst(name, pos, quaternion);
                return result;
            }
            
            result = Object.Instantiate(prefab, pos, quaternion);
            result.name = name;
            
            return result;
        }
        
        public GameObject GetPool(GameObject prefab, Vector3 pos)
        {
            string name = prefab.name;
        
            if (!_pools.ContainsKey(name))
                _pools[name] = new LinkedList<object>();

            GameObject result;
            var quaternion = prefab.transform.rotation;

            if(_pools[name].Count > 0)
            {
                result = GetPoolFirst(name, pos, quaternion);
                return result;
            }
            
            result = Object.Instantiate(prefab, pos, quaternion);
            result.name = name;
            
            return result;
        }
        
        private GameObject GetPoolFirst(string name, Vector3 pos, Quaternion quaternion)
        {
            GameObject result = _pools[name].First.Value is GameObject 
                ? _pools[name].First.Value as GameObject 
                : (_pools[name].First.Value as MonoBehaviour).gameObject;
            
            result.transform.rotation = quaternion;
            result.transform.position = pos;
            result.SetActive(true);
            _pools[name].RemoveFirst();

            return result;
        }

    #endregion

    public void PoolPreLaunch<T>(T prefab, int count, Transform container) where T : MonoBehaviour
    {
        string name = prefab.gameObject.name;
        if (!_pools.ContainsKey(name))
            _pools[name] = new LinkedList<object>();

        T result;

        for (int i = 0; i < count; i++)
        {
            result = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            result.gameObject.name = name;
        
            _pools[name].AddFirst(result);
            result.gameObject.SetActive(false);
            result.transform.SetParent(container);
        }
    }
    
    public void PoolPreLaunch(GameObject prefab, int count, Transform container)
    {
        string name = prefab.gameObject.name;
        if (!_pools.ContainsKey(name))
            _pools[name] = new LinkedList<object>();

        GameObject result;

        for (int i = 0; i < count; i++)
        {
            result = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            result.name = name;
        
            _pools[name].AddFirst(result);
            result.SetActive(false);
            result.transform.SetParent(container);
        }
    }
    
    public void SetPool(GameObject target)
    {
        if (!_pools.ContainsKey(target.name))
        {
            Object.Destroy(target);
            return;
        }
        
        _pools[target.name].AddFirst(target);
        target.SetActive(false);
    }
    
    public void SetPool(Transform target)
    {
        if (!_pools.ContainsKey(target.name))
        {
            Object.Destroy(target);
            return;
        }
        
        _pools[target.name].AddFirst(target.gameObject);
        target.gameObject.SetActive(false);
    }
    
    public void SetPool<T>(T target) where T: MonoBehaviour
    {
        if (!_pools.ContainsKey(target.gameObject.name))
        {
            Object.Destroy(target.gameObject);
            return;
        }
        
        _pools[target.name].AddFirst(target);
        target.gameObject.SetActive(false);
    }	
    
    public void ClearPool()
    {
        _pools.Clear();
    }
}
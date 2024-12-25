using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Utils
{
    public class RandomModelActivator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _models = new List<GameObject>();
        
        private GameObject _current;

        public GameObject GetModel()
        {
            var rnd = Random.Range(0, _models.Count);
            _current = _models[rnd];
            for (var i = 0; i < _models.Count; i++)
            {
                _models[i].SetActive(i == rnd);
            }

            return _current;
        }

        [Button]
        private void SetRandom()
        {
            var rnd = Random.Range(0, _models.Count);
            _current = _models[rnd];
            for (var i = 0; i < _models.Count; i++)
            {
                _models[i].SetActive(i == rnd);
            }
        }
    }
}
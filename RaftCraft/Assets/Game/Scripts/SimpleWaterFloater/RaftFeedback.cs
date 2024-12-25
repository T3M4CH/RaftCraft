using System.Collections.Generic;
using UnityEngine;

public class RaftFeedback : MonoBehaviour
{
    [SerializeField] private float _amplitude = 0.2f;
    [SerializeField] private float _length = 0.4f;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _gravity = -10f;
    [SerializeField] private Vector2 _initialForceMinMax = new Vector2(3f, 20f);
    [SerializeField] private ParticleSystem _particleSystem;
    
    private float _offset;
    private List<WaterFloater> _floaters = new List<WaterFloater>();
    private bool _isDownScale;
    private float _timer;
    private EnemyRaft _enemyRaft;

    public void Crumble()
    {
        var floaters = GetComponentsInChildren<WaterFloater>(true);
        for (int i = 0; i < floaters.Length; i++)
        {
            _floaters.Add(floaters[i]);
            var dir = Quaternion.Euler(0f, Random.Range(-60f, 60f), 0f) * (floaters[i].Position - transform.position);
            floaters[i].Setup(dir * Random.Range(0.5f, 0.8f), _amplitude, _length, _gravity, Random.Range(_initialForceMinMax.x, _initialForceMinMax.y));
        }
        
        _particleSystem.Play();
    }

    public void DestroyParts(EnemyRaft enemyRaft)
    {
        _enemyRaft = enemyRaft;
        _isDownScale = true;
        _timer = 1.2f;
    }
    
    private void Update()
    {
        _offset = Time.time * _speed;
        
        foreach (var floater in _floaters)
            floater.UpdateHeight(_offset);

        TimerScale();
    }

    private void TimerScale()
    {
        if(_isDownScale == false) return;

        _timer = Mathf.MoveTowards(_timer, 0f, Time.deltaTime);
        foreach (var floater in _floaters)
            floater.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, _timer);

        if (_timer == 0f)
        {
            Destroy(_enemyRaft.gameObject);
        }
    }
}

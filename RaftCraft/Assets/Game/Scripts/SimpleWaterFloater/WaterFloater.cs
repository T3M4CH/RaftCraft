using UnityEngine;
using Random = UnityEngine.Random;

public class WaterFloater : MonoBehaviour
{
    [SerializeField] private float _depthBeforeSubmerged = 1f;
    [SerializeField] private float _displacementAmount = 6f;
    [SerializeField] private float _waterDrag = 1f;
    
    private float _speedFloater;
    private float _speedRotate;
    private float _amplitude;
    private float _length;
    private float _gravity;
    private Vector3 _dir;
    private Quaternion _targetRotate;

    public Vector3 Position => transform.position;
    
    public void Setup(Vector3 dir, float amplitude, float length, float gravity, float speedFloater)
    {
        _amplitude = amplitude;
        _length = length;
        _gravity = gravity;
        _dir = dir;
        _speedRotate = Random.Range(-50f, 50f);
        _speedFloater = speedFloater;
        _targetRotate = Random.rotation;
    }
    
    public void UpdateHeight(float time)
    {
        var delta = Time.deltaTime;
        var waveHeight = _amplitude * Mathf.Sin(transform.position.x * _length + time);

        var displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / _depthBeforeSubmerged) * _displacementAmount;

        if (transform.position.y < waveHeight)
        {
            _speedFloater += displacementMultiplier * delta;
            _speedFloater = Mathf.Lerp(_speedFloater, 0f, _waterDrag * delta);
        }
        else
        {
            _speedFloater += _gravity * delta;
        }

        transform.position += new Vector3(_dir.x, _speedFloater, _dir.z) * delta;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotate, _speedRotate * delta);
    }
}
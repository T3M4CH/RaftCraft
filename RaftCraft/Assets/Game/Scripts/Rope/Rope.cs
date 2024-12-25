using DG.Tweening;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private float _durationGrab = 0.4f;
    
    private LineRenderer _lineRenderer;
    private Tween _tween;
    
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void GrabToPoint(Vector3 point)
    {
        var currentPoint = transform.position;
        _lineRenderer.SetPosition(0, currentPoint);
        _lineRenderer.SetPosition(1, currentPoint);

        _tween = DOTween
            .To(() => currentPoint, x => currentPoint = x, point, _durationGrab)
            .OnUpdate(() =>
            {
                _lineRenderer.SetPosition(1, currentPoint);
            });
    }

    private void OnDestroy()
    {
        _tween.Kill();
    }
}

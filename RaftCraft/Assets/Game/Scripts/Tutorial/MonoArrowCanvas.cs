using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class MonoArrowCanvas : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float amplitude;
    [SerializeField] private RectTransform arrowImage;

    private float _startPosition;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Ease _ease;
    private Sequence _sequence;

    private void Start()
    {
        StartSequence();
    }

    [Button]
    private void StartSequence()
    {
        _sequence.Kill();

        arrowImage.localPosition = Vector3.zero;
        _sequence = DOTween.Sequence();
        _sequence.Append(_canvasGroup.DOFade(1, 0.5f).From(0));
        _sequence.Append(arrowImage.DOMove(arrowImage.position - arrowImage.up * amplitude, duration).SetEase(_ease).SetLoops(int.MaxValue, LoopType.Yoyo));
    }

    private void OnEnable()
    {
        StartSequence();
    }

    private void OnDisable()
    {
        _sequence.Kill();
    }
    
    [field:SerializeField] public TMP_Text ArrowText { get; private set; }
}

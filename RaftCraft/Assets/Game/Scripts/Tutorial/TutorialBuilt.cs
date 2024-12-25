using System;
using UnityEngine;
using Game.Scripts.Extension;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using UnityEngine.UI;

public class TutorialBuilt : MonoBehaviour
{
    public event Action OnComplete = () => {};
    
    [SerializeField] private float _delay;
    [SerializeField] private Image _progress;
    [SerializeField] private LayerMask _layerMask;
    
    private int currentCount;
    private float _currentTime;
    private IResourceService _resourceService;
    
    private const int TargetCount = 3;

    public void Initialize(IResourceService resourceService)
    {
        transform.parent.gameObject.SetActive(true);
        _currentTime = _delay;
        _resourceService = resourceService;
        
        // _panelEffect.DOKill();
        // _panelEffect.DOScale(_sizePush, _duration).SetEase(_ease).OnComplete(() =>
        // {
        //     _panelEffect.DOScale(Vector3.one, _duration).SetEase(_ease);
        // });
    }

    private void OnTriggerStay(Collider other)
    {
        if (_layerMask.Includes(other.gameObject.layer))
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime < 0)
            {
                _currentTime = _delay;
                _resourceService.TryRemoveLocal(EResourceType.Wood, 1);
                currentCount += 1;

                _progress.fillAmount = (float)currentCount / TargetCount;
                if (currentCount >= TargetCount)
                {
                    OnComplete.Invoke();
                    
                    transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _currentTime = _delay;
    }
}
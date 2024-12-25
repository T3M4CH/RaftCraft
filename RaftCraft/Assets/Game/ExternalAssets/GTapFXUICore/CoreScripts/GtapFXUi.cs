using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GtapFXUi
{
    public enum  TypeEmojis
    {
        Positive,
        Negative
    }
    
    [SerializeField] private GtapFxData _data;

    private Transform _canvas;

    private GameObject[] _positiveEmojis;

    private GameObject[] _negativeEmojis;

    private bool _isInit;

    public bool IsInit
    {
        private set
        {
            _isInit = value;
        }
        get
        {
            if (!_isInit)
            {
                Init();
            }

            return _isInit;
        }
    }
    
    private void Init()
    {
        if(_isInit) return;
        _data = (GtapFxData)Resources.Load("GtapFXUiData");
        if (_data == null)
        {
            Debug.LogError("GtapFxUi: " + "GtapFxUi Not Loaded == null");
        }
        
        _canvas = Object.Instantiate(_data.CanvasPrefab).transform;
        
        IsInit = true;
    }

    private GameObject _currentEmojis;
    
    public void ShowEmojis(TypeEmojis typeEmojis)
    {
        switch (typeEmojis)
        {
            case TypeEmojis.Positive:
                if (_currentEmojis != null)
                {
                    Object.Destroy(_currentEmojis);
                }
                var positiveEmojy = _data.PositiveEmojies[Random.Range(0, _data.PositiveEmojies.Length)];
                _currentEmojis = Object.Instantiate(positiveEmojy, _canvas);
                _currentEmojis.transform.localPosition = _data.PositionSpawn;
                Object.Destroy(_currentEmojis, 3f);
                break;
            case TypeEmojis.Negative:
                if (_currentEmojis != null)
                {
                    Object.Destroy(_currentEmojis);
                }
                var negativaEmojy = _data.NegativaEmojies[Random.Range(0, _data.PositiveEmojies.Length)];
                _currentEmojis = Object.Instantiate(negativaEmojy, _canvas);
                _currentEmojis.transform.localPosition = _data.PositionSpawn;
                Object.Destroy(_currentEmojis, 3f);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(typeEmojis), typeEmojis, null);
        }
    }
}

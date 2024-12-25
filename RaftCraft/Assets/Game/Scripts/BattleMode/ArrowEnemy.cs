using Game.Scripts.Extension;
using UnityEngine;
using UnityEngine.UI;

public class ArrowEnemy : MonoBehaviour
{
    [SerializeField] private float _offsetBorder;
    [SerializeField] private float _offsetBottom;
    [SerializeField] private float _offsetTop;
    [SerializeField] private float _verticalOffset;
    [SerializeField] private Image _icon;
    
    private Transform _target;
    private RectTransform _rectTransform;
    private Camera _camera;
    private Vector4 _border;
    private EnemyRaft _raft;

    private void Start()
    {
        _border = new Vector4(_offsetBorder, Screen.width - _offsetBorder, _offsetBottom, Screen.height - _offsetTop);
        _camera = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if(_camera == null) return;

        if (_target == null || _target.gameObject.activeSelf == false)
        {
            _target = _raft.GetArrowTarget();
            return;
        }

        var worldPos = (_target.position + Vector3.up * _verticalOffset);

        var screenPosition = _camera.WorldToScreenPoint(worldPos);
        var position = screenPosition;
        if (screenPosition.z < 0f)
        {
            screenPosition.x *= -1f;
            screenPosition.y *= -1f;
        }

        if (screenPosition.x < 0f || screenPosition.x > Screen.width || screenPosition.y < 0f || screenPosition.y > Screen.height)
        {
            _icon.gameObject.SetActive(true);
        }
        else
        {
            _icon.gameObject.SetActive(false);
        }
        
        screenPosition.x = Mathf.Clamp(screenPosition.x, _border.x, _border.y);
        screenPosition.y = Mathf.Clamp(screenPosition.y, _border.z, _border.w);
        
        _rectTransform.position = Vector3.Lerp(_rectTransform.position, screenPosition, Time.smoothDeltaTime * 10f);
        var rotZ = (position - _rectTransform.position).Angle();
        _rectTransform.eulerAngles = new Vector3(0f, 0f, rotZ);
    }

    public void Setup(EnemyRaft raft)
    {
        _raft = raft;
    }
}
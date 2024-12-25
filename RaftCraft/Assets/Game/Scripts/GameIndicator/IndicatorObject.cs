using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Scripts.GameIndicator
{
    public class IndicatorObject : MonoBehaviour
    {
        [SerializeField] private RectTransform _mainPanelRect;
        [SerializeField, FoldoutGroup("UI")] private RectTransform _rect;
        [SerializeField, FoldoutGroup("UI")] private RectTransform _rotate;
        [SerializeField, FoldoutGroup("UI")] private Image _imageIcon;
        [SerializeField, FoldoutGroup("UI")] private CanvasGroup _canvasGroup;

        public Vector2 _offset;
        private Vector2 _position;

        private Camera _main;

        private bool _isRotate;
        private bool _isStaticPosition;
        private float _distanceThreshold;
        private float _multiplierOffset;
        private RectTransform _parentRectArea;
        private Transform _playerTransform;

        [SerializeField]
        private Transform _target;

        private void Start()
        {
            _main = Camera.main;
        }

        public void SetPlayer(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        public void SetTarget(Transform target, Vector3 position,float distanceThreshold, Sprite sprite, Color color, bool isStaticPosition = false, float multiplierOffset = 3f)
        {
            _target = target;
            _offset = position;
            _rect.position = position;
            _imageIcon.sprite = sprite;
            _imageIcon.color = color;
            _distanceThreshold = distanceThreshold;
            _isStaticPosition = isStaticPosition;
            _multiplierOffset = multiplierOffset;

            //TODO: Стоит куда-нибудь вынести
            _parentRectArea = transform.parent.parent.GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!_isStaticPosition)
            {
                var position = _main.WorldToScreenPoint(_target.position);

                position.x = Mathf.Clamp(position.x, _rect.rect.width * _multiplierOffset, Screen.width - (_rect.rect.width * _multiplierOffset));
                position.y = Mathf.Clamp(position.y, _rect.rect.height * _multiplierOffset, Screen.height - (_rect.rect.height * _multiplierOffset));
                position.z = 0f;

                _rect.position = position;
                _position = position;
            }

            Rotate();
            _canvasGroup.alpha = IsFarEnough() ? 1f : 0f;
        }

        private bool IsFarEnough()
        {
            if (_playerTransform == null) return false;

            return Vector3.Distance(_playerTransform.position, _target.position) > _distanceThreshold;
        }

        private void Rotate()
        {
            var position = _main.WorldToScreenPoint(_target.position);
            position.y -= _position.y;
            position.x -= _position.x;
            position.z = 0f;
            _rotate.eulerAngles = new Vector3(0, 0, Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg);
        }
    }
}
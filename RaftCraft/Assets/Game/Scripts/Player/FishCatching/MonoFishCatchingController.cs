using Game.Scripts.NPC;
using Game.Scripts.NPC.Fish.Systems;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Player;
using UnityEngine;

public class MonoFishCatchingController : MonoBehaviour
{
    [SerializeField] private float fadingSpeed;
    [SerializeField] private EntityPlayer _entityPlayer;
    [SerializeField] private Transform _zoneTransform;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Color _activeColor;

    private Color _clearColor;
    private float _fillValue;
    private Transform _playerTransform;
    private FishingSystem _weaponController;
    
    private void FixedUpdate()
    {
        if (_entityPlayer.Space != LocationSpace.Water)
        {
            _sprite.color = _clearColor;
            return;
        }
        
        var targetTransform = _weaponController.TargetBaseFish != null ? _weaponController.TargetBaseFish.transform : null;
        if (targetTransform)
        {
            _fillValue += Time.deltaTime * fadingSpeed;
            var direction = targetTransform.position - _playerTransform.position;
            direction = direction.normalized;

            _zoneTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg);
        }
        else
        {
            _fillValue -= Time.deltaTime * fadingSpeed;
        }

        _fillValue = Mathf.Clamp(_fillValue, 0, 1);
        _sprite.color = Color.Lerp(_clearColor, _activeColor, _fillValue);
    }

    private void Start()
    {
        var col = _activeColor;
        col.a = 0;
        _clearColor = col;
        _playerTransform = _entityPlayer.Hips;
        _weaponController = _entityPlayer.FishingSystem;
        transform.SetParent(_playerTransform);
    }
}
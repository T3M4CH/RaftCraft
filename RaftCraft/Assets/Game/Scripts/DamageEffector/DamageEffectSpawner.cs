using System;
using Game.Scripts.DamageEffector.Data;
using Game.Scripts.Health;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Pool;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Game.Scripts.DamageEffector
{
    public class DamageEffectSpawner
    {
        private SerializableHealthBarSettings _data;
        private Camera _main;
        private RectTransform _parent;
        private PoolObjects<DamageText> _poolTextPirate;
        private PoolObjects<DamageText> _poolTextPlayer;

        public DamageEffectSpawner(EffectTextDamageData textPrefab, SerializableDamageEffectSettings dataCanvas)
        {
            _main = Camera.main;
            _parent = dataCanvas.Panel;
            _poolTextPirate = new PoolObjects<DamageText>(textPrefab.DamageTextPirate, dataCanvas.Parent, 10);
            _poolTextPlayer = new PoolObjects<DamageText>(textPrefab.DamageTextPlayer, dataCanvas.Parent, 10);
        }

        public void Spawn(Vector3 position, float damage, bool critical = false, EntityType type = EntityType.Pirate)
        {
            _main ??= Camera.main;
            
            var text = GetObject(type);
            text.SetValue(damage);
            text.SetCritical(critical);
            text.Rect.localPosition = Position(position);
            text.gameObject.SetActive(true);
            text.StartMove();
        }

        private Vector3 Position(Vector3 inputPosition)
        {
            var rnd = new Vector2(Random.Range(-25f, 25f), 0f);
            return (inputPosition.WorldToScreenPosition(_main, _parent) + (Vector2.up) * 70f) + rnd;
        }

        private DamageText GetObject(EntityType type)
        {
            switch (type)
            {
                case EntityType.Player:
                    return _poolTextPlayer.GetFree();
                case EntityType.Fish:
                    return _poolTextPirate.GetFree();
                case EntityType.Pirate:
                    return _poolTextPirate.GetFree();
                case EntityType.TileRaft:
                    return _poolTextPirate.GetFree();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
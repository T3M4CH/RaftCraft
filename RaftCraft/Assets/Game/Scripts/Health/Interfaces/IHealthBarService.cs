using Game.Scripts.NPC.Interface;
using UnityEngine;

namespace Game.Scripts.Health.Interfaces
{
    public interface IHealthBarService
    {
        void SetLevel(Transform transform, int value);
        void SetValue(Transform transform, IHeals value);

        void RemovePanel(Transform transform);
    }
}
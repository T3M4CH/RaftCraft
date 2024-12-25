using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.TransitionEffect
{
    [Serializable]
    public struct SerializableTransitionSettings
    {
        [field:SerializeField] public Image DarkeningSprite { get; private set; }
        //TODO: Добавить другие настройки
    }
}
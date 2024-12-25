using System;

namespace Game.Scripts.GameSettings
{
    public interface IUpdateGameSettings
    {
        event Action<bool> OnChangeMusic; 
        event Action<bool> OnChangeHaptic;
        event Action<bool> OnChangeSound;
        
        bool Music { get; }
        bool Sound { get; }
        bool Haptic { get; }
    }
}

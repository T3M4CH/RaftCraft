using Game.Scripts.TransitionEffect.Enums;

namespace Game.Scripts.TransitionEffect.Interfaces
{
    public interface ITransitionEffectService
    {
        void ShowDarkening(float duration, EZoomType zoomType = EZoomType.None, float multiplier = 0);
    }
}
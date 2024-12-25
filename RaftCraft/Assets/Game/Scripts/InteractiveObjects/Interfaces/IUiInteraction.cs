using Game.Scripts.InteractiveObjects.Interfaces;
using Game.Scripts.UI.WindowManager;

namespace Game.Scripts.InteractiveObjects
{
    public interface IUiInteraction : IInteraction
    {
        public void OpenWindow(WindowManager windowManager);
    }
}
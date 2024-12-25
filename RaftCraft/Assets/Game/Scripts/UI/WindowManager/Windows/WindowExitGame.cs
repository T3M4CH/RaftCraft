using Game.Scripts.Joystick.Extras;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Scripts.UI.WindowManager.Windows
{
    public class WindowExitGame : UIWindow
    {
        private InputSingleton _inputSingleton;
        
        [Inject]
        private void Construct(InputSingleton inputSingleton)
        {
            _inputSingleton = inputSingleton;
            
            _inputSingleton.Instance.Player.Back.performed += _ =>
            {
                WindowManager.OpenWindow<WindowExitGame>();
            }; 
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }

        public void Close()
        {
            WindowManager.CloseWindow<WindowExitGame>();
        }
        
    }
}

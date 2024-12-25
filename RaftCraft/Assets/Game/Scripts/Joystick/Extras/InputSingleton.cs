namespace Game.Scripts.Joystick.Extras
{
    public class InputSingleton
    {
        private PlayerInput _instance;

        public PlayerInput Instance => _instance;
        public InputSingleton()
        {
            _instance = new PlayerInput();
            _instance.Enable();
        }
    }
}
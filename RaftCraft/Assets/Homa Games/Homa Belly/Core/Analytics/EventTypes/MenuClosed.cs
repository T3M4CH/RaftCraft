namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class MenuClosed : SessionEvent
    {
        public string MenuName { get; }
        public MenuClosed(string menuName)
        {
            MenuName = menuName;
        }
    }
}
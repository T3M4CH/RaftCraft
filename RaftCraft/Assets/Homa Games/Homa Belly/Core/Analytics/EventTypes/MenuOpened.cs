namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class MenuOpened : SessionEvent
    {
        public string MenuName { get; }
        public MenuOpened(string menuName)
        {
            MenuName = menuName;
        }
    }
}
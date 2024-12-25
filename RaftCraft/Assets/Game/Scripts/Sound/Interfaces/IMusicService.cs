namespace Game.Scripts.Sound.Interfaces
{
    public interface IMusicService
    {
        void StopMusic();
        void PlayMusic(EMusicType musicType, float? delay = null);
    }
}
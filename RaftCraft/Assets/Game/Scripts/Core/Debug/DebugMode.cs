namespace Game.Scripts.Core.Debug
{
    [System.Flags]
    public enum DebugMode
    {
        Log = 1 << 0,
        LogWarning = 1 << 1,
        LogError = 1 << 2,
        All = Log | LogWarning | LogError
    }
}

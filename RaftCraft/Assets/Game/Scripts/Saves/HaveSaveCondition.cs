[System.Flags]
public enum HaveSaveCondition
{
    HaveFocus = 1 << 1,
    HavePause = 1 << 2,
    HaveDestroy = 1 << 3,
    HaveQuit = 1 << 4,
    HaveDisable = 1 << 5,
    All = HaveFocus | HavePause | HaveDestroy | HaveQuit | HaveDisable
}
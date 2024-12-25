namespace Game.Scripts.Player.EntityGame
{
    [System.Flags]
    public enum EntityType 
    {
        Player = 1 << 0,
        Fish =  1 << 1,
        Pirate = 1 << 2,
        TileRaft = 1 << 3
    }
}

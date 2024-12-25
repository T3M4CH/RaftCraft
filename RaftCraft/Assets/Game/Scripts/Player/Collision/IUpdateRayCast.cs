namespace Game.Scripts.Player.Collision
{
    public interface IUpdateRayCast
    {
        public bool HaveListener { get; }
        public void UpdateCast(CastData data);
    }
}

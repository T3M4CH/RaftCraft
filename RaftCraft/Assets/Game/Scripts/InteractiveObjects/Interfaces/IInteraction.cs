namespace Game.Scripts.InteractiveObjects.Interfaces
{
    public interface IInteraction
    {
        public bool IsAbleEverywhere { get; }
        public bool Interaction { get; }
        public InteractionType CurrentTypeInteraction { get; }
        
        public float DelayAction { get; }

        public void Action();

        public void EnterInteraction();

        void ExitInteraction();
    }
}

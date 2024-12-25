namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class TutorialStepComplete : TutorialEvent
    {
        public long StepDuration { get; }
        public TutorialStepComplete(int tutorialStep, long stepDuration) : base(tutorialStep)
        {
            StepDuration = stepDuration;
        }
    }
}
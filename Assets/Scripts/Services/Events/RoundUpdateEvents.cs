using Services.Loaders.Configs;

namespace Services.Events
{
    public class RoundCounterUpdatedEvent : GameEventBase
    {
        public int CurrentRound { get; private set; }
        public int RoundsCount { get; private set; }
        public RoundCounterUpdatedEvent(int currentRound, int roundsCount)
        {
            CurrentRound = currentRound;
            RoundsCount = roundsCount;
        }
    }

    public class TaskUpdatedEvent : GameEventBase
    {
        public ColorDefinition TargetColor { get; private set; }
        public TaskUpdatedEvent(ColorDefinition targetColorDefinition)
        {
            TargetColor = targetColorDefinition;
        }
    }
}
namespace Reflex.Core
{
    public sealed class LevelDefinition
    {
        public int Id { get; }
        public float DurationSeconds { get; }

        // Probability weights (0–1)
        public float PositiveChance { get; }
        public float NegativeChance { get; }
        public float MultiplierChance { get; }
        public float DividerChance { get; }
        public float AddTimeChance { get; }
        public float SubtractTimeChance { get; }

        public LevelDefinition(
            int id,
            float durationSeconds,
            float positive,
            float negative,
            float multiplier,
            float divider,
            float addTime,
            float subtractTime)
        {
            Id = id;
            DurationSeconds = durationSeconds;

            PositiveChance = positive;
            NegativeChance = negative;
            MultiplierChance = multiplier;
            DividerChance = divider;
            AddTimeChance = addTime;
            SubtractTimeChance = subtractTime;
        }
    }
}

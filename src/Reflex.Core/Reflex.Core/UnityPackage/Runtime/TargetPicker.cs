namespace Reflex.Core
{
    public sealed class TargetPicker
    {
        private readonly IGameRng _rng;

        public TargetPicker(IGameRng rng)
        {
            _rng = rng;
        }

        public TargetKind NextTarget(LevelDefinition level)
        {
            float roll = _rng.NextFloat01();
            float acc = 0f;

            acc += level.PositiveChance;
            if (roll < acc) return TargetKind.AddScore_Positive;

            acc += level.NegativeChance;
            if (roll < acc) return TargetKind.AddScore_Negative;

            acc += level.MultiplierChance;
            if (roll < acc) return TargetKind.MultiplyScore_x2;

            acc += level.DividerChance;
            if (roll < acc) return TargetKind.DivideScore_div2;

            acc += level.AddTimeChance;
            if (roll < acc) return TargetKind.AddTime;

            return TargetKind.SubtractTime;
        }

        private TargetKind PickOne(params TargetKind[] kinds)
            => kinds[_rng.NextInt(0, kinds.Length)];
    }
}
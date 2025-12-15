namespace Reflex.Core
{
    public sealed class TargetPicker
    {
        private readonly IGameRng _rng;

        public TargetPicker(IGameRng rng)
        {
            _rng = rng;
        }

        public TargetKind NextTarget(float badChance)
        {
            // Decide "good" vs "bad"
            bool isBad = _rng.NextFloat01() < badChance;

            if (!isBad)
            {
                // Good pool
                return PickOne(
                    TargetKind.AddScore_Positive,
                    TargetKind.MultiplyScore_x2,
                    TargetKind.AddTime
                );
            }

            // Bad pool
            return PickOne(
                TargetKind.AddScore_Negative,
                TargetKind.DivideScore_div2,
                TargetKind.SubtractTime
            );
        }

        private TargetKind PickOne(params TargetKind[] kinds)
            => kinds[_rng.NextInt(0, kinds.Length)];
    }
}
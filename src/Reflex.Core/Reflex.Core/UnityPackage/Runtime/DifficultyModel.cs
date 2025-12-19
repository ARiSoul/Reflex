using System;

namespace Reflex.Core
{
    public sealed class DifficultyModel
    {
        // Tune these to taste
        public float MinSpawnInterval = 0.35f;
        public float MaxSpawnInterval = 1f;
        public float MinSpeed = 1f;
        public float MaxSpeed = 10f;

        // 0..1 over run duration
        public float NormalizedProgress(float elapsed, float totalDuration)
            => totalDuration <= 0f ? 1f : MathF.Min(1f, elapsed / totalDuration);

        public float SpawnInterval(float t01)
            => Lerp(MaxSpawnInterval, MinSpawnInterval, EaseIn(t01));

        public float TargetSpeed(float t01)
            => Lerp(MinSpeed, MaxSpeed, EaseIn(t01));

        // Probability of "bad" targets increases a bit over time
        public float BadTargetChance(float t01)
            => Lerp(0.20f, 0.40f, t01);

        private static float Lerp(float a, float b, float t) => a + (b - a) * t;
        private static float EaseIn(float t) => t * t; // simple curve
    }
}

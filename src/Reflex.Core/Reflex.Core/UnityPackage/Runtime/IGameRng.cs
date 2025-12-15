using System;

namespace Reflex.Core
{
    public interface IGameRng
    {
        int NextInt(int minInclusive, int maxExclusive);
        float NextFloat01();
    }

    public sealed class SystemRng : IGameRng
    {
        private readonly Random _rng;

        public SystemRng(int seed)
        {
            _rng = new Random(seed);
        }

        public int NextInt(int minInclusive, int maxExclusive) => _rng.Next(minInclusive, maxExclusive);
        public float NextFloat01() => (float)_rng.NextDouble();
    }
}

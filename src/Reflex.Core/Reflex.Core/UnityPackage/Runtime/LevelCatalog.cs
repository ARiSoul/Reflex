using System.Collections.Generic;

namespace Reflex.Core
{
    public static class LevelCatalog
    {
        public static IReadOnlyList<LevelDefinition> Levels { get; } =
            new List<LevelDefinition>
            {
                new LevelDefinition(
                    id: 1,
                    durationSeconds: 30,
                    positive: 0.70f,
                    negative: 0.10f,
                    multiplier: 0.10f,
                    divider: 0.00f,
                    addTime: 0.10f,
                    subtractTime: 0.00f),

                new LevelDefinition(
                    id: 2,
                    durationSeconds: 30,
                    positive: 0.60f,
                    negative: 0.15f,
                    multiplier: 0.10f,
                    divider: 0.05f,
                    addTime: 0.05f,
                    subtractTime: 0.05f),

                new LevelDefinition(
                    id: 3,
                    durationSeconds: 35,
                    positive: 0.50f,
                    negative: 0.20f,
                    multiplier: 0.15f,
                    divider: 0.05f,
                    addTime: 0.05f,
                    subtractTime: 0.05f),

                new LevelDefinition(
                    id: 4,
                    durationSeconds: 37,
                    positive: 0.45f,
                    negative: 0.25f,
                    multiplier: 0.20f,
                    divider: 0.06f,
                    addTime: 0.04f,
                    subtractTime: 0.06f),

                new LevelDefinition(
                    id: 5,
                    durationSeconds: 39,
                    positive: 0.42f,
                    negative: 0.28f,
                    multiplier: 0.22f,
                    divider: 0.07f,
                    addTime: 0.03f,
                    subtractTime: 0.07f),

                new LevelDefinition(
                    id: 6,
                    durationSeconds: 43,
                    positive: 0.40f,
                    negative: 0.30f,
                    multiplier: 0.25f,
                    divider: 0.08f,
                    addTime: 0.03f,
                    subtractTime: 0.08f),

                new LevelDefinition(
                    id: 7,
                    durationSeconds: 45,
                    positive: 0.37f,
                    negative: 0.33f,
                    multiplier: 0.28f,
                    divider: 0.09f,
                    addTime: 0.25f,
                    subtractTime: 0.09f),

                new LevelDefinition(
                    id: 8,
                    durationSeconds: 48,
                    positive: 0.35f,
                    negative: 0.35f,
                    multiplier: 0.30f,
                    divider: 0.1f,
                    addTime: 0.2f,
                    subtractTime: 0.1f),

                new LevelDefinition(
                    id: 9,
                    durationSeconds: 50,
                    positive: 0.33f,
                    negative: 0.37f,
                    multiplier: 0.33f,
                    divider: 0.15f,
                    addTime: 0.08f,
                    subtractTime: 0.12f),

                new LevelDefinition(
                    id: 10,
                    durationSeconds: 55,
                    positive: 0.30f,
                    negative: 0.40f,
                    multiplier: 0.40f,
                    divider: 0.2f,
                    addTime: 0.05f,
                    subtractTime: 0.15f),
            };
    }
}

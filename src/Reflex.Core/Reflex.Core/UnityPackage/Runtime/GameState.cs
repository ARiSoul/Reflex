using System;

namespace Reflex.Core
{
    public sealed class GameState
    {
        public int Score { get; private set; }
        public float TimeLeft { get; private set; }
        public int Combo { get; private set; }
        public int Mistakes { get; private set; }
        public bool IsGameOver => TimeLeft <= 0f || Mistakes >= MaxMistakes;

        public int MaxMistakes { get; }

        public GameState(float startTimeSeconds, int maxMistakes)
        {
            TimeLeft = startTimeSeconds;
            MaxMistakes = maxMistakes;
        }

        public void Tick(float deltaSeconds)
        {
            if (IsGameOver) return;
            TimeLeft = MathF.Max(0f, TimeLeft - deltaSeconds);
        }

        public void ApplyHit(TargetKind kind)
        {
            if (IsGameOver) return;

            switch (kind)
            {
                case TargetKind.AddScore_Positive:
                    AddScore(50);
                    Combo++;
                    break;

                case TargetKind.AddScore_Negative:
                    AddScore(-30);
                    Mistakes++;
                    Combo = 0;
                    break;

                case TargetKind.MultiplyScore_x2:
                    Score *= 2;
                    Combo++;
                    break;

                case TargetKind.DivideScore_div2:
                    Score /= 2;
                    Mistakes++;
                    Combo = 0;
                    break;

                case TargetKind.AddTime:
                    TimeLeft = MathF.Min(TimeLeft + 3f, 99f);
                    Combo++;
                    break;

                case TargetKind.SubtractTime:
                    TimeLeft = MathF.Max(TimeLeft - 3f, 0f);
                    Mistakes++;
                    Combo = 0;
                    break;
            }

            // Optional: combo bonus (small but satisfying)
            if (Combo > 0 && Combo % 5 == 0)
                AddScore(25);
        }

        private void AddScore(int delta)
        {
            Score = Math.Max(0, Score + delta);
        }
    }
}

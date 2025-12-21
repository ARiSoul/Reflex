using System;

namespace Reflex.Core
{
    public sealed class GameState
    {
        public long Score { get; private set; }
        public float TimeLeft { get; private set; }
        public int Combo { get; private set; }
        public int Mistakes { get; private set; }

        public int CurrentLevel { get; set; } = 1;

        public int MaxMistakes { get; }
        public int PlayerValue { get; private set; }

        // Main fail condition for this style of game:
        public bool IsGameOver => PlayerValue <= 0 || Mistakes >= MaxMistakes;

        public GameState(float startTimeSeconds, int maxMistakes, int startPlayerValue)
        {
            TimeLeft = startTimeSeconds;
            MaxMistakes = maxMistakes;
            PlayerValue = Math.Max(0, startPlayerValue);
        }

        public void Tick(float deltaSeconds)
        {
            if (IsGameOver)
                return;

            TimeLeft = MathF.Max(0f, TimeLeft - deltaSeconds);
        }

        public void ApplyHit(TargetKind kind)
        {
            if (IsGameOver)
                return;

            switch (kind)
            {
                case TargetKind.AddScore_Positive:
                    AddValue(5);
                    Combo++;
                    break;

                case TargetKind.AddScore_Negative:
                    AddValue(-5);
                    Mistakes++;
                    Combo = 0;
                    break;

                case TargetKind.MultiplyScore_x2:
                    PlayerValue = ApplySoftMultiply(PlayerValue);
                    Combo++;
                    break;

                case TargetKind.DivideScore_div2:
                    PlayerValue = ClampValue(PlayerValue > 50000 ? PlayerValue / 3 : PlayerValue / 2);
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

            // Optional: give score too (secondary)
            // This keeps your existing HUD stuff meaningful.
            if (Combo > 0)
                AddScore(10);

            // Optional: small combo bonus
            if (Combo > 0 && Combo % 5 == 0)
                AddScore(25);
        }

        private void AddValue(int delta)
        {
            PlayerValue = ClampValue(PlayerValue + delta);
        }

        private static int ClampValue(int value)
        {
            if (value < 0) return 0;
            if (value > 999999) return 999999;
            return value;
        }

        private void AddScore(long delta)
        {
            Score = Math.Max(0, Score + delta);
        }

        public void ResetTime(float seconds)
        {
            if (seconds < 0f) seconds = 0f;
            TimeLeft = seconds;
        }

        public void ResetMistakes()
        {
            Mistakes = 0;
        }

        public void ResetPlayerValue(int startValue)
        {
            PlayerValue = Math.Max(0, startValue);
        }

        private int ApplySoftMultiply(int value)
        {
            if (value < 10000) return ClampValue(value * 2);
            if (value < 50000) return ClampValue((int)(value * 1.5f));
            if (value < 200000) return ClampValue((int)(value * 1.25f));

            return ClampValue((int)(value * 1.1f));
        }

        public void MissRow()
        {
            if (IsGameOver) return;

            Mistakes++;
            Combo = 0;
        }
    }
}

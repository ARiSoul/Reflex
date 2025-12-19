using Reflex.Core;
using UnityEngine;

public static class TargetPicker
{
    public static TargetKind Pick(LevelConfig level)
    {
        float total = level.TotalWeight;
        if (total <= 0f)
            return TargetKind.AddScore_Positive;

        float r = Random.value * total;

        r -= Mathf.Max(0f, level.Positive);
        if (r <= 0f) return TargetKind.AddScore_Positive;

        r -= Mathf.Max(0f, level.Negative);
        if (r <= 0f) return TargetKind.AddScore_Negative;

        r -= Mathf.Max(0f, level.Multiplier);
        if (r <= 0f) return TargetKind.MultiplyScore_x2;

        r -= Mathf.Max(0f, level.Divider);
        if (r <= 0f) return TargetKind.DivideScore_div2;

        r -= Mathf.Max(0f, level.AddTime);
        if (r <= 0f) return TargetKind.AddTime;

        return TargetKind.SubtractTime;
    }
}

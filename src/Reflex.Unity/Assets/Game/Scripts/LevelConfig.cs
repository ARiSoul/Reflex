using UnityEngine;

[CreateAssetMenu(menuName = "Reflex/Level Config", fileName = "LevelConfig")]
public sealed class LevelConfig : ScriptableObject
{
    [Header("Identity")]
    public int Id = 1;

    [Header("Timing")]
    public float DurationSeconds = 30f;

    [Header("Gate Weights (don’t need to sum to 1)")]
    public float Positive = 0.70f;
    public float Negative = 0.10f;
    public float Multiplier = 0.10f;
    public float Divider = 0.00f;
    public float AddTime = 0.10f;
    public float SubtractTime = 0.00f;

    [Header("Spawner Tuning (per level)")]
    public float TargetSpeed = 6f;

    [Tooltip("How often to spawn a PAIR (seconds). Will be clamped by spacing rule.")]
    public float SpawnIntervalSeconds = 1.2f;

    [Tooltip("Max chance for both gates to be the same category (good-good or bad-bad). 0 = always good vs bad.")]
    [Range(0f, 1f)]
    public float SameCategoryChance = 0.15f;

    [Tooltip("Bias towards bad when SameCategoryChance triggers. 0 = always good-good, 1 = always bad-bad.")]
    [Range(0f, 1f)]
    public float SameCategoryBadBias = 0.5f;

    [Header("Fairness")]
    [Range(0f, 1f)]
    public float GoodGoodChance = 0.12f;

    [Tooltip("Minimum waves between good+good jackpots.")]
    public int MinWavesBetweenGoodGood = 4;

    [Tooltip("If you haven't seen AddTime in this many waves, force it as the good option.")]
    public int MaxWavesWithoutAddTime = 6;

    [Tooltip("Max times in a row we allow severe bad (Divide/SubtractTime).")]
    public int MaxSevereBadInARow = 2;

    public float TotalWeight =>
        Mathf.Max(0f, Positive) +
        Mathf.Max(0f, Negative) +
        Mathf.Max(0f, Multiplier) +
        Mathf.Max(0f, Divider) +
        Mathf.Max(0f, AddTime) +
        Mathf.Max(0f, SubtractTime);

    public void NormalizeWeights()
    {
        float total = TotalWeight;
        if (total <= 0f)
            return;

        Positive = Mathf.Max(0f, Positive) / total;
        Negative = Mathf.Max(0f, Negative) / total;
        Multiplier = Mathf.Max(0f, Multiplier) / total;
        Divider = Mathf.Max(0f, Divider) / total;
        AddTime = Mathf.Max(0f, AddTime) / total;
        SubtractTime = Mathf.Max(0f, SubtractTime) / total;
    }
}

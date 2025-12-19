using Reflex.Core;
using TMPro;
using UnityEngine;

public sealed class HudView : MonoBehaviour
{
    [Header("References")]
    public TMP_Text ScoreText;
    public TMP_Text TimeText;
    public TMP_Text ComboText;
    public TMP_Text MistakesText;
    public TMP_Text CurrentLevelText;
    public LowTimeWarning LowTimeWarning;

    [Header("Juice")]
    public UiPunch ScorePunch;
    public UiPunch ComboPunch;

    private long _lastScore;
    private int _lastCombo;

    public void Render(GameState state)
    {
        if (ScoreText != null) ScoreText.text = $"Score: {state.Score}";
        if (TimeText != null) TimeText.text = $"Time: {state.TimeLeft:0.0}s";
        if (ComboText != null) ComboText.text = $"Combo: {state.Combo}";
        if (MistakesText != null) MistakesText.text = $"Mistakes: {state.Mistakes}/{state.MaxMistakes}";
        if (CurrentLevelText != null) CurrentLevelText.text = $"Level: {state.CurrentLevel}";

        if (state.Score != _lastScore)
        {
            ScorePunch.Punch();
            _lastScore = state.Score;
        }

        if (state.Combo != _lastCombo)
        {
            ComboPunch.Punch();
            _lastCombo = state.Combo;
        }

        LowTimeWarning?.SetTime(state.TimeLeft);
    }
}

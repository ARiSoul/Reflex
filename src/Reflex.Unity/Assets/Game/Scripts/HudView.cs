using Reflex.Core;
using TMPro;
using UnityEngine;

public sealed class HudView : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _comboText;
    [SerializeField] private TMP_Text _mistakesText;
    [SerializeField] private LowTimeWarning _lowTimeWarning;


    [Header("Juice")]
    [SerializeField] private UiPunch _scorePunch;
    [SerializeField] private UiPunch _comboPunch;

    private long _lastScore;
    private int _lastCombo;

    public void Render(GameState state)
    {
        if (_scoreText != null) _scoreText.text = $"Score: {state.Score}";
        if (_timeText != null) _timeText.text = $"Time: {state.TimeLeft:0.0}s";
        if (_comboText != null) _comboText.text = $"Combo: {state.Combo}";
        if (_mistakesText != null) _mistakesText.text = $"Mistakes: {state.Mistakes}/{state.MaxMistakes}";

        if (state.Score != _lastScore)
        {
            _scorePunch.Punch();
            _lastScore = state.Score;
        }

        if (state.Combo != _lastCombo)
        {
            _comboPunch.Punch();
            _lastCombo = state.Combo;
        }

        _lowTimeWarning?.SetTime(state.TimeLeft);
    }
}

using TMPro;
using UnityEngine;

public sealed class LowTimeWarning : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private float _thresholdSeconds = 5f;

    private float _pulseT;
    private bool _active;

    private void Awake()
    {
        if (_timeText == null) _timeText = GetComponent<TMP_Text>();
    }

    public void SetTime(float timeLeft)
    {
        _active = timeLeft > 0f && timeLeft <= _thresholdSeconds;
    }

    private void Update()
    {
        if (_timeText == null) return;

        if (!_active)
        {
            // restore full alpha
            var c = _timeText.color; c.a = 1f; _timeText.color = c;
            return;
        }

        _pulseT += Time.unscaledDeltaTime * 8f;
        float a = 0.4f + Mathf.Abs(Mathf.Sin(_pulseT)) * 0.6f;

        var col = _timeText.color;
        col.a = a;
        _timeText.color = col;
    }
}

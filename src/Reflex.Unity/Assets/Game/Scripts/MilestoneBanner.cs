using TMPro;
using UnityEngine;

public sealed class MilestoneBanner : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _showSeconds = 0.6f;
    [SerializeField] private float _punchStrength = 0.35f;

    private float _t;
    private bool _active;
    private Vector3 _baseScale;

    private void Awake()
    {
        if (_text == null) _text = GetComponent<TMP_Text>();
        _baseScale = transform.localScale;
        Hide();
    }

    public void Show(string message)
    {
        if (_text == null) return;

        _text.text = message;
        _text.gameObject.SetActive(true);

        _t = 0f;
        _active = true;
        transform.localScale = _baseScale * 0.85f;
        var c = _text.color; c.a = 1f; _text.color = c;
    }

    public void Hide()
    {
        if (_text != null) _text.gameObject.SetActive(false);
        _active = false;
    }

    private void Update()
    {
        if (!_active || _text == null) return;

        _t += Time.unscaledDeltaTime;
        float p = Mathf.Clamp01(_t / _showSeconds);

        // Punch
        float wave = Mathf.Sin(p * Mathf.PI);
        transform.localScale = _baseScale * (1f + wave * _punchStrength);

        // Fade out near end
        var c = _text.color;
        c.a = 1f - p;
        _text.color = c;

        if (p >= 1f)
            Hide();
    }
}

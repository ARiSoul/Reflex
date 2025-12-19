using TMPro;
using UnityEngine;

public sealed class MilestoneBanner : MonoBehaviour
{
    public TMP_Text Text;
    public float ShowSeconds = 0.6f;
    public float PunchStrength = 0.35f;

    private float _t;
    private bool _active;
    private Vector3 _baseScale;

    private void Awake()
    {
        if (Text == null) Text = GetComponent<TMP_Text>();
        _baseScale = transform.localScale;
        Hide();
    }

    public void Show(string message)
    {
        if (Text == null) return;

        Text.text = message;
        Text.gameObject.SetActive(true);

        _t = 0f;
        _active = true;
        transform.localScale = _baseScale * 0.85f;
        var c = Text.color; c.a = 1f; Text.color = c;
    }

    public void Hide()
    {
        if (Text != null) Text.gameObject.SetActive(false);
        _active = false;
    }

    private void Update()
    {
        if (!_active || Text == null) return;

        _t += Time.unscaledDeltaTime;
        float p = Mathf.Clamp01(_t / ShowSeconds);

        // Punch
        float wave = Mathf.Sin(p * Mathf.PI);
        transform.localScale = _baseScale * (1f + wave * PunchStrength);

        // Fade out near end
        var c = Text.color;
        c.a = 1f - p;
        Text.color = c;

        if (p >= 1f)
            Hide();
    }
}

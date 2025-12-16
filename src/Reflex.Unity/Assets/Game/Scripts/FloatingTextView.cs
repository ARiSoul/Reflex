using TMPro;
using UnityEngine;

public sealed class FloatingTextView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private float _life;
    private float _age;
    private Vector3 _velocity;
    private Color _baseColor;

    public void Play(string message, Color color, Vector3 startPos, Vector3 velocity, float lifetime)
    {
        if (_text == null) _text = GetComponent<TMP_Text>();

        transform.position = startPos;
        _velocity = velocity;
        _life = Mathf.Max(0.01f, lifetime);
        _age = 0f;

        _baseColor = color;
        _text.text = message;
        _text.color = color;

        gameObject.SetActive(true);
        transform.localScale = Vector3.one; // reset
    }

    private void Update()
    {
        _age += Time.unscaledDeltaTime;

        // move (unscaled so it still moves during hitstop)
        transform.position += _velocity * Time.unscaledDeltaTime;

        // tiny scale pop at start
        float pop = 1f + Mathf.Sin(Mathf.Clamp01(_age / 0.12f) * Mathf.PI) * 0.15f;
        transform.localScale = Vector3.one * pop;

        // fade out
        float t = Mathf.Clamp01(_age / _life);
        var c = _baseColor;
        c.a = 1f - t;
        _text.color = c;

        if (_age >= _life)
            gameObject.SetActive(false);
    }
}

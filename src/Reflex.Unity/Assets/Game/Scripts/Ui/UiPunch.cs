using UnityEngine;

public sealed class UiPunch : MonoBehaviour
{
    [SerializeField] private float _strength = 0.18f;
    [SerializeField] private float _duration = 0.12f;

    private Vector3 _baseScale;
    private float _t;
    private bool _active;

    private void Awake()
    {
        _baseScale = transform.localScale;
    }

    public void Punch()
    {
        _t = 0f;
        _active = true;
    }

    private void Update()
    {
        if (!_active) return;

        _t += Time.unscaledDeltaTime;
        float p = Mathf.Clamp01(_t / _duration);

        // up and back down
        float wave = Mathf.Sin(p * Mathf.PI);
        transform.localScale = _baseScale * (1f + wave * _strength);

        if (p >= 1f)
        {
            transform.localScale = _baseScale;
            _active = false;
        }
    }
}

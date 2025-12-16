using UnityEngine;
using Reflex.Core;
using System;
using TMPro;

public sealed class TargetView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _col;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private float _spawnPunchStrength = 0.22f;
    [SerializeField] private float _spawnPunchDuration = 0.10f;
    [SerializeField] private float _wobbleAmount = 0.04f;
    [SerializeField] private float _wobbleSpeed = 7f;

    public TargetKind Kind { get; private set; }
    public event Action<TargetView> OnDespawned;

    private float _speed;
    private Vector2 _dir;
    private float _spawnT;
    private Vector3 _baseScale;
    private float _wobbleSeed;

    public void Init(TargetKind kind, float speed, Vector2 direction, Sprite sprite)
    {
        Kind = kind;
        _speed = speed;
        _dir = direction.normalized;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.color = ColorFor(kind);
        }

        if (_label != null)
            _label.text = LabelFor(kind);

        if (_col != null) _col.enabled = true;

        gameObject.SetActive(true);

        _baseScale = Vector3.one;
        transform.localScale = _baseScale * 0.85f; // start slightly smaller
        _spawnT = 0f;
        _wobbleSeed = UnityEngine.Random.value * 1000f;
    }

    private void Update()
    {
        transform.position += (Vector3)(_speed * Time.deltaTime * _dir);

        // spawn punch
        _spawnT += Time.deltaTime;
        float p = Mathf.Clamp01(_spawnT / _spawnPunchDuration);
        float wave = Mathf.Sin(p * Mathf.PI);
        float punch = 0.85f + wave * _spawnPunchStrength; // grows then settles
        transform.localScale = _baseScale * Mathf.Lerp(punch, 1f, p);

        // subtle wobble (makes targets feel alive)
        float wobble = Mathf.Sin((Time.time + _wobbleSeed) * _wobbleSpeed) * _wobbleAmount;
        transform.rotation = Quaternion.Euler(0f, 0f, wobble * 10f);
    }

    public void Despawn()
    {
        if (_col != null) _col.enabled = false;

        OnDespawned?.Invoke(this);
    }

    private static Color ColorFor(TargetKind kind) => kind switch
    {
        TargetKind.AddScore_Positive => Color.green,
        TargetKind.AddScore_Negative => Color.red,
        TargetKind.MultiplyScore_x2 => Color.yellow,
        TargetKind.DivideScore_div2 => new Color(0.65f, 0.35f, 1f),
        TargetKind.AddTime => Color.cyan,
        TargetKind.SubtractTime => new Color(1f, 0.55f, 0.05f),
        _ => Color.white
    };

    private static string LabelFor(TargetKind kind) => kind switch
    {
        TargetKind.AddScore_Positive => "+50",
        TargetKind.AddScore_Negative => "-30",
        TargetKind.MultiplyScore_x2 => "x2",
        TargetKind.DivideScore_div2 => "÷2",
        TargetKind.AddTime => "+3s",
        TargetKind.SubtractTime => "-3s",
        _ => "?"
    };
}

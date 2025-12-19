using UnityEngine;
using Reflex.Core;
using System;
using TMPro;

public sealed class GateView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _col;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private float _spawnPunchStrength = 0.22f;
    [SerializeField] private float _spawnPunchDuration = 0.10f;
    [SerializeField] private float _wobbleAmount = 0.04f;
    [SerializeField] private float _wobbleSpeed = 7f;

    public TargetKind Kind { get; private set; }

    // Assigned by spawner so it can despawn the sibling gate.
    public int PairId { get; set; }

    public event Action<GateView> OnDespawned;
    public event Action<GateView> OnChosen;

    private float _speed;
    private float _spawnT;
    private Vector3 _baseScale;
    private float _wobbleSeed;

    private bool _isDespawned;
    private bool _isChosen;

    public void Init(TargetKind kind, float speed, Sprite sprite)
    {
        Kind = kind;
        _speed = speed;

        _isDespawned = false;
        _isChosen = false;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.color = ColorFor(kind);
        }

        if (_label != null)
            _label.text = LabelFor(kind);

        if (_col != null)
            _col.enabled = true;

        _baseScale = Vector3.one;
        transform.localScale = _baseScale * 0.85f;
        _spawnT = 0f;
        _wobbleSeed = UnityEngine.Random.value * 1000f;
        transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        if (_isDespawned)
            return;

        transform.position += Vector3.down * (_speed * Time.deltaTime);

        _spawnT += Time.deltaTime;
        float p = Mathf.Clamp01(_spawnT / _spawnPunchDuration);
        float wave = Mathf.Sin(p * Mathf.PI);
        float punch = 0.85f + wave * _spawnPunchStrength;
        transform.localScale = _baseScale * Mathf.Lerp(punch, 1f, p);

        float wobble = Mathf.Sin((Time.time + _wobbleSeed) * _wobbleSpeed) * _wobbleAmount;
        transform.rotation = Quaternion.Euler(0f, 0f, wobble * 10f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDespawned)
            return;

        if (!other.CompareTag("Player"))
            return;

        Choose();
    }

    private void Choose()
    {
        if (_isChosen || _isDespawned)
            return;

        _isChosen = true;
        OnChosen?.Invoke(this);

        Despawn();
    }

    public void ForceDespawn()
    {
        if (_isDespawned)
            return;

        _isChosen = true;
        Despawn();
    }

    public void Despawn()
    {
        if (_isDespawned)
            return;

        _isDespawned = true;

        if (_col != null)
            _col.enabled = false;

        // Don't deactivate here: pool will do that in Release(view)
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

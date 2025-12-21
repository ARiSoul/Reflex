using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Reflex.Core;

public sealed class GateView : MonoBehaviour
{
    [FormerlySerializedAs("_spriteRenderer")]
    public SpriteRenderer SpriteRenderer;

    [FormerlySerializedAs("_col")]
    public Collider2D Col;

    [FormerlySerializedAs("_label")]
    public TMP_Text Label;

    [FormerlySerializedAs("_spawnPunchStrength")]
    public float SpawnPunchStrength = 0.22f;

    [FormerlySerializedAs("_spawnPunchDuration")]
    public float SpawnPunchDuration = 0.10f;

    [FormerlySerializedAs("_wobbleAmount")]
    public float WobbleAmount = 0.04f;

    [FormerlySerializedAs("_wobbleSpeed")]
    public float WobbleSpeed = 7f;

    public TargetKind Kind { get; private set; }

    // Assigned by spawner so it can despawn the sibling gate.
    public int PairId { get; set; }

    // True only if the PLAYER chose this gate (entered trigger).
    public bool WasChosen { get; private set; }

    public event Action<GateView> OnDespawned;
    public event Action<GateView> OnChosen;

    private float _speed;
    private float _spawnT;
    private Vector3 _baseScale;
    private float _wobbleSeed;
    private bool _isDespawned;

    public void Init(TargetKind kind, float speed, Sprite sprite)
    {
        Kind = kind;
        _speed = speed;

        _isDespawned = false;
        WasChosen = false;

        if (SpriteRenderer != null)
        {
            SpriteRenderer.sprite = sprite;
            SpriteRenderer.color = ColorFor(kind);
        }

        if (Label != null)
            Label.text = LabelFor(kind);

        if (Col != null)
            Col.enabled = true;

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
        float p = SpawnPunchDuration <= 0f ? 1f : Mathf.Clamp01(_spawnT / SpawnPunchDuration);
        float wave = Mathf.Sin(p * Mathf.PI);
        float punch = 0.85f + wave * SpawnPunchStrength;
        transform.localScale = _baseScale * Mathf.Lerp(punch, 1f, p);

        float wobble = Mathf.Sin((Time.time + _wobbleSeed) * WobbleSpeed) * WobbleAmount;
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
        if (WasChosen || _isDespawned)
            return;

        WasChosen = true;
        OnChosen?.Invoke(this);

        Despawn();
    }

    // Used when sibling must disappear or bounds cleanup happens.
    // IMPORTANT: this is NOT a player choice, so WasChosen stays false.
    public void ForceDespawn()
    {
        if (_isDespawned)
            return;

        Despawn();
    }

    public void Despawn()
    {
        if (_isDespawned)
            return;

        _isDespawned = true;

        if (Col != null)
            Col.enabled = false;

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
        TargetKind.AddScore_Positive => "+5",
        TargetKind.AddScore_Negative => "-5",
        TargetKind.MultiplyScore_x2 => "x2",
        TargetKind.DivideScore_div2 => "÷2",
        TargetKind.AddTime => "+3s",
        TargetKind.SubtractTime => "-3s",
        _ => "?"
    };
}

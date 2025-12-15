using UnityEngine;
using Reflex.Core;
using System;

public sealed class TargetView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _col;

    public TargetKind Kind { get; private set; }
    public event Action<TargetView> OnDespawned;

    private float _speed;
    private Vector2 _dir;

    public void Init(TargetKind kind, float speed, Vector2 direction, Sprite sprite)
    {
        Kind = kind;
        _speed = speed;
        _dir = direction.normalized;

        if (_spriteRenderer != null) _spriteRenderer.sprite = sprite;
        if (_col != null) _col.enabled = true;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += (Vector3)(_speed * Time.deltaTime * _dir);
    }

    public void Despawn()
    {
        if (_col != null) _col.enabled = false;

        OnDespawned?.Invoke(this);
    }
}

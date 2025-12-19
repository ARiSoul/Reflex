using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerController : MonoBehaviour
{
    [Header("Lanes")]
    public int LaneCount = 3;
    public float LaneWidth = 2f;
    public float CenterX = 0f;

    [Header("Lane Move Feel")]
    public float LaneChangeSpeed = 35f; // higher = snappier
    public bool InstantSnap = false;

    [Header("Shooting")]
    public Transform BulletSpawnPoint;
    public GameObject BulletPrefab;
    public float BulletSpeed = 10f;

    [Header("Input Actions")]
    public InputActionReference MoveAction; // Player/Move (Vector2)
    public InputActionReference FireAction; // Player/Fire (Button)

    private Rigidbody2D _rb;

    private int _laneIndex;
    private float _targetX;
    private int _lastMoveSign;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        LaneCount = Mathf.Max(1, LaneCount);

        _laneIndex = Mathf.Clamp(LaneCount / 2, 0, LaneCount - 1);
        _targetX = GetLaneX(_laneIndex);

        var pos = _rb.position;
        pos.x = _targetX;
        _rb.position = pos;
    }

    private void OnEnable()
    {
        MoveAction.action.Enable();
        FireAction.action.Enable();

        MoveAction.action.performed += OnMove;
        MoveAction.action.canceled += OnMove;

        FireAction.action.performed += OnFire;
    }

    private void OnDisable()
    {
        MoveAction.action.performed -= OnMove;
        MoveAction.action.canceled -= OnMove;

        FireAction.action.performed -= OnFire;

        MoveAction.action.Disable();
        FireAction.action.Disable();
    }

    private void FixedUpdate()
    {
        // Lane movement only (X). Y stays as physics/camera dictates.
        var pos = _rb.position;

        if (InstantSnap)
        {
            pos.x = _targetX;
            _rb.MovePosition(pos);
            return;
        }

        pos.x = Mathf.MoveTowards(pos.x, _targetX, LaneChangeSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(pos);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        var move = ctx.ReadValue<Vector2>();
        var sign = GetMoveSign(move.x);

        if (sign != 0 && _lastMoveSign == 0)
            ChangeLane(sign);

        _lastMoveSign = sign;
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        Shoot();
    }

    private void ChangeLane(int direction)
    {
        _laneIndex = Mathf.Clamp(_laneIndex + direction, 0, LaneCount - 1);
        _targetX = GetLaneX(_laneIndex);
    }

    private float GetLaneX(int laneIndex)
    {
        var half = (LaneCount - 1) * 0.5f;
        var offset = (laneIndex - half) * LaneWidth;
        return CenterX + offset;
    }

    private static int GetMoveSign(float x)
    {
        if (x > 0.5f) return 1;
        if (x < -0.5f) return -1;
        return 0;
    }

    private void Shoot()
    {
        if (BulletPrefab == null || BulletSpawnPoint == null)
            return;

        var bullet = Instantiate(BulletPrefab, BulletSpawnPoint.position, BulletSpawnPoint.rotation);

        var bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb == null)
            return;

        bulletRb.AddForce(BulletSpawnPoint.up * BulletSpeed, ForceMode2D.Impulse);
    }
}

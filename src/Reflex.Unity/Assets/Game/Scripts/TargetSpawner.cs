using Reflex.Core;
using UnityEngine;

public sealed class TargetSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TargetView _targetPrefab;
    [SerializeField] private Transform _poolParent;

    [Header("Spawn Area")]
    [SerializeField] private Transform _leftSpawn;
    [SerializeField] private Transform _rightSpawn;

    [Header("Sprites by Kind")]
    [SerializeField] private Sprite _addPositive;
    [SerializeField] private Sprite _addNegative;
    [SerializeField] private Sprite _multX2;
    [SerializeField] private Sprite _div2;
    [SerializeField] private Sprite _addTime;
    [SerializeField] private Sprite _subTime;

    private ObjectPool<TargetView> _pool;

    private DifficultyModel _difficulty;
    private TargetPicker _picker;

    private float _elapsed;
    private float _nextSpawnAt;
    private float _runDuration = 45f;

    public void Init(int seed, DifficultyModel difficulty, float runDurationSeconds)
    {
        _pool = new ObjectPool<TargetView>(_targetPrefab, prewarm: 24, parent: _poolParent);

        _difficulty = difficulty;
        _picker = new TargetPicker(new SystemRng(seed));

        _elapsed = 0f;
        _runDuration = runDurationSeconds;
        _nextSpawnAt = 0f;
    }

    public void TickSpawner(float dt)
    {
        _elapsed += dt;

        if (_elapsed >= _nextSpawnAt)
        {
            SpawnOne();
            float t01 = _difficulty.NormalizedProgress(_elapsed, _runDuration);
            _nextSpawnAt = _elapsed + _difficulty.SpawnInterval(t01);
        }
    }

    private void SpawnOne()
    {
        float t01 = _difficulty.NormalizedProgress(_elapsed, _runDuration);
        float badChance = _difficulty.BadTargetChance(t01);
        float speed = _difficulty.TargetSpeed(t01);

        var kind = _picker.NextTarget(badChance);
        var sprite = SpriteFor(kind);

        // spawn left -> right or right -> left
        bool fromLeft = Random.value < 0.5f;
        Vector3 spawnPos = fromLeft ? _leftSpawn.position : _rightSpawn.position;
        Vector2 dir = fromLeft ? Vector2.right : Vector2.left;

        var target = _pool.Get();
        target.transform.position = spawnPos;
        target.Init(kind, speed, dir, sprite);
    }

    private Sprite SpriteFor(TargetKind kind) => kind switch
    {
        TargetKind.AddScore_Positive => _addPositive,
        TargetKind.AddScore_Negative => _addNegative,
        TargetKind.MultiplyScore_x2 => _multX2,
        TargetKind.DivideScore_div2 => _div2,
        TargetKind.AddTime => _addTime,
        TargetKind.SubtractTime => _subTime,
        _ => _addPositive
    };

    // Call this from a boundary cleaner or when targets go offscreen.
    public void Release(TargetView view) => _pool.Release(view);
}

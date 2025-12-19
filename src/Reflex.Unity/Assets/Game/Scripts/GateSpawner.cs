using Reflex.Core;
using System.Collections.Generic;
using UnityEngine;

public sealed class GateSpawner : MonoBehaviour
{
    [Header("References")]
    public GateView GatePrefab;
    public Transform PoolParent;

    [Header("Lanes")]
    public int LaneCount = 3;
    public float LaneWidth = 2f;
    public float CenterX = 0f;

    [Header("Spawn Spacing (prevents overlap)")]
    public float GateHeightWorldUnits = 2.0f;
    public float MinGapWorldUnits = 0.5f;

    [Header("Sprites by Kind")]
    public Sprite AddPositive;
    public Sprite AddNegative;
    public Sprite MultX2;
    public Sprite Div2;
    public Sprite AddTime;
    public Sprite SubTime;

    [Header("Game")]
    public LevelManager LevelManager;

    private ObjectPool<GateView> _pool;

    private float _elapsed;
    private float _nextSpawnAt;

    private int _nextPairId = 1;
    private readonly Dictionary<int, (GateView A, GateView B)> _pairs = new();

    private int _activeGates;
    public bool HasActiveGates => _activeGates > 0;
    public bool IsSpawningEnabled = true;

    public void Init()
    {
        _pool = new ObjectPool<GateView>(GatePrefab, prewarm: 24, parent: PoolParent);

        _elapsed = 0f;
        _nextSpawnAt = 0f;

        LaneCount = Mathf.Max(1, LaneCount);
        GateHeightWorldUnits = Mathf.Max(0.01f, GateHeightWorldUnits);
        MinGapWorldUnits = Mathf.Max(0f, MinGapWorldUnits);

        _pairs.Clear();
        _nextPairId = 1;
        _activeGates = 0;
    }

    public void TickSpawner(float dt)
    {
        if (LevelManager == null || LevelManager.CurrentLevel == null)
            return;

        if (!IsSpawningEnabled)
            return;

        _elapsed += dt;

        if (_elapsed < _nextSpawnAt)
            return;

        var level = LevelManager.CurrentLevel;

        SpawnPair(level);

        float interval = Mathf.Max(0.01f, level.SpawnIntervalSeconds);
        float speed = Mathf.Max(0.01f, level.TargetSpeed);

        float minInterval = (GateHeightWorldUnits + MinGapWorldUnits) / speed;
        _nextSpawnAt = _elapsed + Mathf.Max(interval, minInterval);
    }

    private void SpawnPair(LevelConfig level)
    {
        float speed = Mathf.Max(0.01f, level.TargetSpeed);

        if (LaneCount < 2)
        {
            SpawnSingle(level, speed);
            return;
        }

        int laneA = Random.Range(0, LaneCount);
        int laneB = Random.Range(0, LaneCount - 1);
        if (laneB >= laneA) laneB++;

        // Default: good vs bad
        var goodKind = PickFromCategory(level, wantGood: true);
        var badKind = PickFromCategory(level, wantGood: false);

        var kindA = Random.value < 0.5f ? badKind : goodKind;
        var kindB = kindA == badKind ? goodKind : badKind;

        // Sometimes allow same-category pair
        if (Random.value < level.SameCategoryChance)
        {
            bool bothBad = Random.value < level.SameCategoryBadBias;

            if (bothBad)
            {
                kindA = PickFromCategory(level, wantGood: false);
                kindB = PickFromCategory(level, wantGood: false);
            }
            else
            {
                kindA = PickFromCategory(level, wantGood: true);
                kindB = PickFromCategory(level, wantGood: true);
            }
        }

        int pairId = _nextPairId++;

        var gateA = SpawnGateAtLane(laneA, speed, kindA, pairId);
        var gateB = SpawnGateAtLane(laneB, speed, kindB, pairId);

        _pairs[pairId] = (gateA, gateB);
    }

    private void SpawnSingle(LevelConfig level, float speed)
    {
        int lane = Random.Range(0, LaneCount);
        var kind = TargetPickerUnity.Pick(level);

        int pairId = _nextPairId++;
        var gate = SpawnGateAtLane(lane, speed, kind, pairId);

        _pairs[pairId] = (gate, null);
    }

    private TargetKind PickFromCategory(LevelConfig level, bool wantGood)
    {
        // Keep trying a few times to satisfy category without overengineering.
        for (int i = 0; i < 8; i++)
        {
            var k = TargetPickerUnity.Pick(level);
            if (TargetPickerUnity.IsGood(k) == wantGood)
                return k;
        }

        // Fallbacks if the weights basically don't allow the requested category.
        return wantGood ? TargetKind.AddScore_Positive : TargetKind.AddScore_Negative;
    }

    private GateView SpawnGateAtLane(int laneIndex, float speed, TargetKind kind, int pairId)
    {
        var sprite = SpriteFor(kind);

        float laneX = GetLaneX(laneIndex);
        Vector3 spawnPosition = new(laneX, transform.position.y, transform.position.z);

        var gate = _pool.Get();

        gate.OnDespawned -= HandleDespawned;
        gate.OnDespawned += HandleDespawned;

        gate.OnChosen -= HandleChosen;
        gate.OnChosen += HandleChosen;

        gate.PairId = pairId;
        gate.transform.position = spawnPosition;
        gate.Init(kind, speed, sprite);

        _activeGates++;

        return gate;
    }

    private void HandleChosen(GateView chosen)
    {
        int pairId = chosen.PairId;

        if (_pairs.TryGetValue(pairId, out var pair))
        {
            if (pair.A != null && pair.A != chosen)
                pair.A.ForceDespawn();

            if (pair.B != null && pair.B != chosen)
                pair.B.ForceDespawn();

            _pairs.Remove(pairId);
        }

        GameManager.Instance.OnGateCollected(chosen);
    }

    private void HandleDespawned(GateView view)
    {
        view.OnDespawned -= HandleDespawned;
        view.OnChosen -= HandleChosen;

        _activeGates = Mathf.Max(0, _activeGates - 1);
        _pool.Release(view);
        view.ForceDespawn();
    }

    private float GetLaneX(int laneIndex)
    {
        var half = (LaneCount - 1) * 0.5f;
        var offset = (laneIndex - half) * LaneWidth;
        return CenterX + offset;
    }

    private Sprite SpriteFor(TargetKind kind) => kind switch
    {
        TargetKind.AddScore_Positive => AddPositive,
        TargetKind.AddScore_Negative => AddNegative,
        TargetKind.MultiplyScore_x2 => MultX2,
        TargetKind.DivideScore_div2 => Div2,
        TargetKind.AddTime => AddTime,
        TargetKind.SubtractTime => SubTime,
        _ => AddPositive
    };

    public void ResetForStage()
    {
        _elapsed = 0f;
        _nextSpawnAt = 0f;
    }

    public void Release(GateView view)
    {
        if (view == null)
            return;

        // This will trigger OnDespawned -> HandleDespawned -> _pool.Release(view)
        view.ForceDespawn();
    }

}

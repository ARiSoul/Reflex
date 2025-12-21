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

    [Header("Cruel Waves")]
    [Range(0f, 1f)]
    public float BadBadChance = 0.18f; // ~1 in 5–6 rows

    [Header("Sprites by Kind")]
    public Sprite AddPositive;
    public Sprite AddNegative;
    public Sprite MultX2;
    public Sprite Div2;
    public Sprite AddTime;
    public Sprite SubTime;

    [Header("Game")]
    public LevelManager LevelManager;

    public bool IsSpawningEnabled = true;
    public bool HasActiveGates => _activeGates > 0;

    private ObjectPool<GateView> _pool;

    private float _elapsed;
    private float _nextSpawnAt;

    private int _nextPairId = 1;

    private int _activeGates;

    private readonly Dictionary<int, (GateView A, GateView B)> _pairs = new();
    private readonly HashSet<int> _resolvedRows = new();
    private readonly Dictionary<int, int> _rowDespawnCount = new();

    // Fairness trackers
    private int _wavesSinceGoodGood;
    private int _wavesSinceAddTime;
    private int _severeBadStreak;

    public void Init()
    {
        _pool = new ObjectPool<GateView>(GatePrefab, prewarm: 24, parent: PoolParent);

        _elapsed = 0f;
        _nextSpawnAt = 0f;
        _resolvedRows.Clear();
        _rowDespawnCount.Clear();

        LaneCount = Mathf.Max(1, LaneCount);
        GateHeightWorldUnits = Mathf.Max(0.01f, GateHeightWorldUnits);
        MinGapWorldUnits = Mathf.Max(0f, MinGapWorldUnits);

        _pairs.Clear();
        _nextPairId = 1;

        _activeGates = 0;

        _wavesSinceGoodGood = 999;
        _wavesSinceAddTime = 999;
        _severeBadStreak = 0;

        IsSpawningEnabled = true;
    }

    public void ResetForStage()
    {
        _elapsed = 0f;
        _nextSpawnAt = 0f;

        _wavesSinceGoodGood = 999;
        _wavesSinceAddTime = 999;
        _severeBadStreak = 0;

        _resolvedRows.Clear();
        _rowDespawnCount.Clear();
        _pairs.Clear();
        _activeGates = 0;
        _nextPairId = 1;
    }

    public void TickSpawner(float dt)
    {
        if (!IsSpawningEnabled)
            return;

        if (LevelManager == null || LevelManager.CurrentLevel == null)
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

        // --- FAIRNESS LAYER: decide the pair kinds ---
        ChoosePairKinds(level, out var kindA, out var kindB);

        int pairId = _nextPairId++;

        var gateA = SpawnGateAtLane(laneA, speed, kindA, pairId);
        var gateB = SpawnGateAtLane(laneB, speed, kindB, pairId);

        _pairs[pairId] = (gateA, gateB);
        _resolvedRows.Remove(pairId);
        _rowDespawnCount[pairId] = 0;
    }

    private void ChoosePairKinds(LevelConfig level, out TargetKind kindA, out TargetKind kindB)
    {
        // defaults: good + bad
        var good = PickGood(level);
        var bad = PickBad(level);

        // Pity: force AddTime if we've gone too long without it (as the GOOD option)
        if (_wavesSinceAddTime >= Mathf.Max(0, level.MaxWavesWithoutAddTime))
            good = TargetKind.AddTime;

        // Jackpot: allow good+good sometimes, but not too often
        bool canJackpot = _wavesSinceGoodGood >= Mathf.Max(0, level.MinWavesBetweenGoodGood);
        bool doJackpot = canJackpot && Random.value < Mathf.Clamp01(level.GoodGoodChance);

        if (doJackpot)
        {
            kindA = PickGood(level);
            kindB = PickGood(level);

            // randomize sides a bit
            if (Random.value < 0.5f)
                (kindA, kindB) = (kindB, kindA);

            _wavesSinceGoodGood = 0;

            // update add-time tracker
            if (kindA == TargetKind.AddTime || kindB == TargetKind.AddTime)
                _wavesSinceAddTime = 0;
            else
                _wavesSinceAddTime++;

            // jackpot wave should reset severe-bad streak pressure a bit
            _severeBadStreak = 0;

            return;
        }

        // Never bad+bad, so we stay good+bad always here.
        // Also avoid "severe bad" too many times in a row (÷2 / -time).
        bool badIsSevere = bad is TargetKind.DivideScore_div2 or TargetKind.SubtractTime;

        if (badIsSevere)
        {
            _severeBadStreak++;

            if (_severeBadStreak > Mathf.Max(1, level.MaxSevereBadInARow))
            {
                // downgrade to the mild bad option
                bad = TargetKind.AddScore_Negative;
                _severeBadStreak = 0;
            }
        }
        else
        {
            _severeBadStreak = 0;
        }

        // assign sides randomly
        if (Random.value < 0.5f)
        {
            kindA = good;
            kindB = bad;
        }
        else
        {
            kindA = bad;
            kindB = good;
        }

        _wavesSinceGoodGood++;

        if (kindA == TargetKind.AddTime || kindB == TargetKind.AddTime)
            _wavesSinceAddTime = 0;
        else
            _wavesSinceAddTime++;
    }

    private void SpawnSingle(LevelConfig level, float speed)
    {
        int lane = Random.Range(0, LaneCount);
        var kind = TargetPickerUnity.Pick(level);

        int pairId = _nextPairId++;
        var gate = SpawnGateAtLane(lane, speed, kind, pairId);

        _pairs[pairId] = (gate, null);
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

            _resolvedRows.Add(pairId);
            _rowDespawnCount.Remove(pairId);
            _pairs.Remove(pairId);
        }

        GameManager.Instance.OnGateCollected(chosen);
    }

    private void HandleDespawned(GateView view)
    {
        view.OnDespawned -= HandleDespawned;
        view.OnChosen -= HandleChosen;

        _activeGates = Mathf.Max(0, _activeGates - 1);

        int rowId = view.PairId;

        _pool.Release(view);

        // If player chose a gate in this row, we don't count misses.
        if (_resolvedRows.Contains(rowId))
            return;

        if (!_rowDespawnCount.TryGetValue(rowId, out var count))
            return;

        count++;
        _rowDespawnCount[rowId] = count;

        if (count >= 2)
        {
            _rowDespawnCount.Remove(rowId);

            // Both gates despawned without being chosen -> missed row
            GameManager.Instance.OnRowMissed(rowId);
        }
    }

    public void Release(GateView view)
    {
        if (view == null)
            return;

        // Ensure counters & events stay consistent
        view.ForceDespawn();
    }

    private float GetLaneX(int laneIndex)
    {
        var half = (LaneCount - 1) * 0.5f;
        var offset = (laneIndex - half) * LaneWidth;
        return CenterX + offset;
    }

    private TargetKind PickGood(LevelConfig level)
    {
        // Try a few times to pick a "good" based on weights
        for (int i = 0; i < 8; i++)
        {
            var k = TargetPickerUnity.Pick(level);
            if (TargetPickerUnity.IsGood(k))
                return k;
        }

        return TargetKind.AddScore_Positive;
    }

    private TargetKind PickBad(LevelConfig level)
    {
        for (int i = 0; i < 8; i++)
        {
            var k = TargetPickerUnity.Pick(level);
            if (!TargetPickerUnity.IsGood(k))
                return k;
        }

        return TargetKind.AddScore_Negative;
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
}

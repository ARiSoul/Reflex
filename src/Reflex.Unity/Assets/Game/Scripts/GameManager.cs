using Reflex.Core;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TargetSpawner _spawner;
    [SerializeField] private InputShooter _inputShooter;

    [Header("Run Settings")]
    [SerializeField] private float _runDurationSeconds = 45f;
    [SerializeField] private int _maxMistakes = 3;

    private GameState _state;
    private DifficultyModel _difficulty;

    private void Start()
    {
        StartRun();
    }

    private void StartRun()
    {
        int seed = System.Environment.TickCount;

        _state = new GameState(startTimeSeconds: _runDurationSeconds, maxMistakes: _maxMistakes);
        _difficulty = new DifficultyModel();

        _spawner.Init(seed, _difficulty, _runDurationSeconds);
        // TODO: init UI here
    }

    private void Update()
    {
        if (_state.IsGameOver)
        {
            // TODO: show results UI + restart button
            Debug.Log($"GAME OVER | Score={_state.Score} TimeLeft={_state.TimeLeft:0.00} Mistakes={_state.Mistakes}/{_state.MaxMistakes}");
            return;
        }

        float dt = Time.deltaTime;

        _state.Tick(dt);
        _spawner.TickSpawner(dt);

        if (_inputShooter.TryGetHit(out var hit))
        {
            var target = hit.collider.GetComponent<TargetView>();
            if (target != null)
            {
                _state.ApplyHit(target.Kind);

                // TODO: VFX/juice hooks (hit stop, shake, sound)
                target.Despawn();
                // optional: spawner.Release(target) if you want pool managed here
            }
        }

        // TODO: update UI with _state.Score, _state.TimeLeft, _state.Combo, _state.Mistakes
    }
}

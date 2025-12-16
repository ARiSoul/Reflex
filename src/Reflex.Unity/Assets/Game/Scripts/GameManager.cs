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
    [SerializeField] private JuiceController _juice;
    [SerializeField] private HudView _hud;
    [SerializeField] private GameOverView _gameOverView;
    [SerializeField] private SfxPlayer _sfx;
    [SerializeField] private VfxPool _vfx;
    [SerializeField] private FloatingTextPool _floatingText;
    [SerializeField] private MilestoneBanner _milestone;


    private int _lastCombo;
    private bool _gameOverShown;
    private GameState _state;
    private DifficultyModel _difficulty;
    private int _currentStage;
    private float _runElapsed;


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

        if (_hud != null)
            _hud.Render(_state);

        _gameOverShown = false;
        _gameOverView?.Hide();

        _lastCombo = 0;
        _milestone.Hide();

        _currentStage = 1;
        _runElapsed = 0f;
        _milestone?.Show("LEVEL 1");
    }

    private void Update()
    {
        if (_state.IsGameOver)
        {
            // TODO: show results UI + restart button
            Debug.Log($"GAME OVER | Score={_state.Score} TimeLeft={_state.TimeLeft:0.00} Mistakes={_state.Mistakes}/{_state.MaxMistakes}");

            if (_hud != null)
                _hud.Render(_state);

            if (!_gameOverShown)
            {
                _gameOverShown = true;
                _sfx.PlayGameOver();
                if (_gameOverView != null)
                    _gameOverView.Show(_state, StartRun);
            }

            return;
        }

        float dt = Time.deltaTime;

        _state.Tick(dt);
        _spawner.TickSpawner(dt);

        _runElapsed += Time.deltaTime;

        int stage = Mathf.FloorToInt(_runElapsed / 15f) + 1;
        if (stage != _currentStage)
        {
            _currentStage = stage;
            _milestone?.Show($"LEVEL {_currentStage}");

            // Optional: micro juice
            _juice?.GoodHit();
        }


        if (_inputShooter.TryGetHit(out var hit))
            if (hit.collider.TryGetComponent<TargetView>(out var target))
            {
                var kind = target.Kind;

                _state.ApplyHit(kind);

                if (_state.Combo != _lastCombo)
                {
                    _lastCombo = _state.Combo;

                    if (_lastCombo > 0 && _lastCombo % 10 == 0)
                        _milestone.Show($"{_lastCombo} COMBO!");

                    if (_lastCombo == 10 && _state.Mistakes == 0)
                        _milestone?.Show("PERFECT!");
                }

                bool isBad = kind is TargetKind.AddScore_Negative
                    or TargetKind.DivideScore_div2
                    or TargetKind.SubtractTime;

                if (_floatingText != null)
                {
                    string msg = kind switch
                    {
                        TargetKind.AddScore_Positive => "+50",
                        TargetKind.AddScore_Negative => "-30",
                        TargetKind.MultiplyScore_x2 => "x2!",
                        TargetKind.DivideScore_div2 => "÷2",
                        TargetKind.AddTime => "+3s",
                        TargetKind.SubtractTime => "-3s",
                        _ => "!"
                    };

                    Color tint = kind switch
                    {
                        TargetKind.AddScore_Positive => Color.green,
                        TargetKind.AddScore_Negative => Color.red,
                        TargetKind.MultiplyScore_x2 => Color.yellow,
                        TargetKind.DivideScore_div2 => new Color(0.65f, 0.35f, 1f),
                        TargetKind.AddTime => Color.cyan,
                        TargetKind.SubtractTime => new Color(1f, 0.55f, 0.05f),
                        _ => Color.white
                    };

                    _floatingText.Spawn(msg, tint, target.transform.position);
                }

                if (kind == TargetKind.MultiplyScore_x2)
                {
                    _sfx.PlayX2();
                    _juice.GoodHit();
                }
                else if (kind == TargetKind.AddTime)
                {
                    _sfx.PlayAddTime();
                    _juice.GoodHit();
                }
                else if (isBad)
                {
                    _sfx.PlayBadHit();
                    _juice.BadHit();
                }
                else
                {
                    _sfx.PlayGoodHit();
                    _juice.GoodHit();
                }

                if (kind == TargetKind.MultiplyScore_x2)
                    _juice.X2SlowMo();

                // Use the same color logic as TargetView (quick duplication for now)
                var vfxTint = kind switch
                {
                    TargetKind.AddScore_Positive => Color.green,
                    TargetKind.AddScore_Negative => Color.red,
                    TargetKind.MultiplyScore_x2 => Color.yellow,
                    TargetKind.DivideScore_div2 => new Color(0.65f, 0.35f, 1f),
                    TargetKind.AddTime => Color.cyan,
                    TargetKind.SubtractTime => new Color(1f, 0.55f, 0.05f),
                    _ => Color.white
                };

                _vfx.PlayAt(target.transform.position, vfxTint);

                target.Despawn();
            }

        if (_hud != null)
            _hud.Render(_state);
    }
}

using UnityEngine;
using Reflex.Core;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Refs")]
    public GateSpawner GateSpawner;
    public InputShooter InputShooter;
    public Transform PlayerTransform;

    [Header("Run Settings")]
    public int MaxMistakes = 3;

    [Header("UI / Feedback")]
    public JuiceController Juice;
    public HudView Hud;
    public GameOverView GameOverView;
    public SfxPlayer Sfx;
    public VfxPool Vfx;
    public FloatingTextPool FloatingText;
    public MilestoneBanner Milestone;
    public LevelManager LevelManager;
    public PlayerValueView PlayerValueView;

    private int _lastCombo;
    private bool _gameOverShown;

    private GameState _state;

    private bool _isTransitioningLevel;
    private bool _levelEnded;

    public float HighScore = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadPreferences();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartRun();
    }

    public void LoadPreferences()
    {
        HighScore = PlayerPrefs.GetFloat("HighScore", 0f);
    }

    public void SavePreferences()
    {
        PlayerPrefs.SetFloat("HighScore", HighScore);
    }

    private void StartRun()
    {
        // Create ONE state for the whole run (score persists across levels)
        _state = new GameState(startTimeSeconds: 0f, maxMistakes: MaxMistakes, startPlayerValue: 10);

        GateSpawner.Init();
        GateSpawner.IsSpawningEnabled = true;
        GateSpawner.ResetForStage();

        _gameOverShown = false;
        _isTransitioningLevel = false;
        _levelEnded = false;

        GameOverView.Hide();
        Milestone.Hide();

        _lastCombo = 0;

        LevelManager.StartFirstLevel();
        StartLevel();
    }

    private void StartLevel()
    {
        var level = LevelManager.CurrentLevel;

        _state.ResetTime(level.DurationSeconds);
        _state.CurrentLevel = level.Id;

        _levelEnded = false;
        _isTransitioningLevel = false;

        GateSpawner.ResetForStage();
        GateSpawner.IsSpawningEnabled = true;

        Milestone.Show($"LEVEL {level.Id}");
        Juice.GoodHit();
    }

    private void BeginLevelTransition()
    {
        if (_isTransitioningLevel)
            return;

        _isTransitioningLevel = true;

        // Stop new spawns; let existing gates finish
        GateSpawner.IsSpawningEnabled = false;

        // Optional: a small banner / feedback
        Milestone.Show("LEVEL COMPLETE");
    }

    private void CompleteLevelTransitionIfReady()
    {
        // Wait until all spawned gates are gone
        if (GateSpawner.HasActiveGates)
            return;

        // Move to next level (or finish run)
        LevelManager.LoadNextLevel();

        // If you want a “win” state when last level is reached:
        // - If LoadNextLevel clamps to last level, we need a way to detect "already last"
        // Simple approach for now: just restart levels forever or keep last.
        // We'll keep it simple: start the (clamped) level again only if time ended.
        StartLevel();
    }

    private void Update()
    {
        // 1) Game over only by mistakes (after we change GameState.IsGameOver)
        if (_state.IsGameOver)
        {
            Debug.Log($"GAME OVER | Score={_state.Score} TimeLeft={_state.TimeLeft:0.00} Mistakes={_state.Mistakes}/{_state.MaxMistakes}");

            Hud.Render(_state);

            if (!_gameOverShown)
            {
                _gameOverShown = true;
                Sfx.PlayGameOver();
                GameOverView.Show(_state, StartRun);
            }

            return;
        }

        float dt = Time.deltaTime;

        // 2) Normal running
        if (!_isTransitioningLevel)
        {
            _state.Tick(dt);
            GateSpawner.TickSpawner(dt);

            // Level ends when time reaches 0 (NOT game over)
            if (!_levelEnded && _state.TimeLeft <= 0f)
            {
                _levelEnded = true;
                BeginLevelTransition();
            }
        }
        else
        {
            // 3) Transition phase: no spawning, just wait for the screen to clear
            CompleteLevelTransitionIfReady();
        }

        Hud.Render(_state);
        PlayerValueView.Render(_state.PlayerValue);
    }

    public void OnGateCollected(GateView gate)
    {
        var kind = gate.Kind;

        _state.ApplyHit(kind);

        if (_state.Combo != _lastCombo)
        {
            _lastCombo = _state.Combo;

            if (_lastCombo > 0 && _lastCombo % 10 == 0)
                Milestone.Show($"{_lastCombo} COMBO!");

            if (_lastCombo == 10 && _state.Mistakes == 0)
                Milestone.Show("PERFECT!");
        }

        bool isBad = kind is TargetKind.AddScore_Negative
            or TargetKind.DivideScore_div2
            or TargetKind.SubtractTime;

        if (FloatingText != null)
        {
            string msg = kind switch
            {
                TargetKind.AddScore_Positive => "+5",
                TargetKind.AddScore_Negative => "-5",
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

            FloatingText.Spawn(msg, tint, gate.transform.position);
        }

        if (kind == TargetKind.MultiplyScore_x2)
        {
            Sfx.PlayX2();
            Juice.GoodHit();
            Juice.X2SlowMo();
        }
        else if (kind == TargetKind.AddTime)
        {
            Sfx.PlayAddTime();
            Juice.GoodHit();
        }
        else if (isBad)
        {
            Sfx.PlayBadHit();
            Juice.BadHit();
        }
        else
        {
            Sfx.PlayGoodHit();
            Juice.GoodHit();
        }

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

        Vfx.PlayAt(gate.transform.position, vfxTint);
    }

    public void OnRowMissed(int rowId)
    {
        _state.MissRow();
        Sfx.PlayBadHit();
        Juice.BadHit();

        // Optional: floating text “MISS” over player
        FloatingText.Spawn("MISS", Color.red, PlayerTransform.position);
    }

}

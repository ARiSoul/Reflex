using Reflex.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameOverView : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _finalScoreText;
    [SerializeField] private Button _retryButton;

    private System.Action _onRetry;

    private void Awake()
    {
        if (_retryButton != null)
            _retryButton.onClick.AddListener(() => _onRetry?.Invoke());

        Hide();
    }

    public void Show(GameState state, System.Action onRetry)
    {
        _onRetry = onRetry;

        if (_finalScoreText != null)
            _finalScoreText.text = $"Score: {state.Score}";

        if (_panel != null)
            _panel.SetActive(true);
    }

    public void Hide()
    {
        if (_panel != null)
            _panel.SetActive(false);
    }
}

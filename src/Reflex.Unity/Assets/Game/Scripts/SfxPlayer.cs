using UnityEngine;

public sealed class SfxPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _source;

    [Header("Clips")]
    [SerializeField] private AudioClip _goodHit;
    [SerializeField] private AudioClip _badHit;
    [SerializeField] private AudioClip _gameOver;

    [Header("Volumes")]
    [Range(0f, 1f)][SerializeField] private float _goodVolume = 0.8f;
    [Range(0f, 1f)][SerializeField] private float _badVolume = 0.8f;
    [Range(0f, 1f)][SerializeField] private float _gameOverVolume = 0.9f;

    private void Awake()
    {
        if (_source == null)
            _source = GetComponent<AudioSource>();
    }

    public void PlayGoodHit()
    {
        if (_goodHit != null) _source.PlayOneShot(_goodHit, _goodVolume);
    }

    public void PlayBadHit()
    {
        if (_badHit != null) _source.PlayOneShot(_badHit, _badVolume);
    }

    public void PlayGameOver()
    {
        if (_gameOver != null) _source.PlayOneShot(_gameOver, _gameOverVolume);
    }
}

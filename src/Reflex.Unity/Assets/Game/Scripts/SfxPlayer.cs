using UnityEngine;

public sealed class SfxPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _source;

    [Header("Clips")]
    [SerializeField] private AudioClip _goodHit;
    [SerializeField] private AudioClip _badHit;
    [SerializeField] private AudioClip _gameOver;

    [Header("Special Clips")]
    [SerializeField] private AudioClip _x2Hit;
    [SerializeField] private AudioClip _addTimeHit;


    [Header("Volumes")]
    [Range(0f, 1f)][SerializeField] private float _goodVolume = 0.8f;
    [Range(0f, 1f)][SerializeField] private float _badVolume = 0.8f;
    [Range(0f, 1f)][SerializeField] private float _gameOverVolume = 0.9f;
    [Range(0f, 1f)][SerializeField] private float _x2Volume = 0.9f;
    [Range(0f, 1f)][SerializeField] private float _addTimeVolume = 0.9f;

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

    public void PlayX2()
    {
        if (_x2Hit != null)
            _source.PlayOneShot(_x2Hit, _x2Volume);
    }

    public void PlayAddTime()
    {
        if (_addTimeHit != null)
            _source.PlayOneShot(_addTimeHit, _addTimeVolume);
    }
}

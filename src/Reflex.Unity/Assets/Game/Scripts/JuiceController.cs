using System.Collections;
using UnityEngine;

public sealed class JuiceController : MonoBehaviour
{
    [Header("Hit Stop")]
    [SerializeField] private float _goodHitStopSeconds = 0.06f;
    [SerializeField] private float _badHitStopSeconds = 0.03f;

    [Header("Camera Shake")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _goodShakeDuration = 0.10f;
    [SerializeField] private float _badShakeDuration = 0.12f;
    [SerializeField] private float _goodShakeStrength = 0.08f;
    [SerializeField] private float _badShakeStrength = 0.12f;

    private Coroutine _shakeRoutine;
    private Vector3 _cameraOriginalPos;
    private bool _isHitStopping;

    private void Awake()
    {
        if (_cameraTransform == null)
            _cameraTransform = Camera.main.transform;

        _cameraOriginalPos = _cameraTransform.localPosition;
    }

    public void GoodHit()
    {
        DoHitStop(_goodHitStopSeconds);
        DoShake(_goodShakeDuration, _goodShakeStrength);
    }

    public void BadHit()
    {
        DoHitStop(_badHitStopSeconds);
        DoShake(_badShakeDuration, _badShakeStrength);
    }

    private void DoHitStop(float seconds)
    {
        if (_isHitStopping) return;
        StartCoroutine(HitStopRoutine(seconds));
    }

    private IEnumerator HitStopRoutine(float seconds)
    {
        _isHitStopping = true;
        float oldScale = Time.timeScale;
        Time.timeScale = 0f;

        // Wait in real time (independent of timescale)
        yield return new WaitForSecondsRealtime(seconds);

        Time.timeScale = oldScale;
        _isHitStopping = false;
    }

    private void DoShake(float duration, float strength)
    {
        if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
        _shakeRoutine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float x = Random.Range(-strength, strength);
            float y = Random.Range(-strength, strength);
            _cameraTransform.localPosition = _cameraOriginalPos + new Vector3(x, y, 0f);
            yield return null;
        }

        _cameraTransform.localPosition = _cameraOriginalPos;
        _shakeRoutine = null;
    }
}

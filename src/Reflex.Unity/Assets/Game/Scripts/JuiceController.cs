using System.Collections;
using UnityEngine;
using Reflex.Core;

public sealed class JuiceController : MonoBehaviour
{
    [Header("Hit Stop")]
    public float GoodHitStopSeconds = 0.06f;
    public float BadHitStopSeconds = 0.03f;

    [Header("Camera Shake")]
    public Transform CameraTransform;
    public float GoodShakeDuration = 0.10f;
    public float BadShakeDuration = 0.12f;
    public float GoodShakeStrength = 0.08f;
    public float BadShakeStrength = 0.12f;

    [Header("Slow Mo")]
    public float X2SlowMoScale = 0.6f;
    public float X2SlowMoSeconds = 0.12f;

    private Coroutine _slowMoRoutine;
    private Coroutine _shakeRoutine;
    private Vector3 _cameraOriginalPos;
    private bool _isHitStopping;

    private void Awake()
    {
        if (CameraTransform == null)
            CameraTransform = Camera.main.transform;

        _cameraOriginalPos = CameraTransform.localPosition;
    }

    public void PlayFor(TargetKind kind)
    {
        if (IsGood(kind))
        {
            GoodHit();
            if (kind == TargetKind.MultiplyScore_x2)
                X2SlowMo();
        }
        else
        {
            BadHit();
        }
    }

    public void GoodHit()
    {
        DoHitStop(GoodHitStopSeconds);
        DoShake(GoodShakeDuration, GoodShakeStrength);
    }

    public void BadHit()
    {
        DoHitStop(BadHitStopSeconds);
        DoShake(BadShakeDuration, BadShakeStrength);
    }

    private static bool IsGood(TargetKind kind) => kind switch
    {
        TargetKind.AddScore_Positive => true,
        TargetKind.MultiplyScore_x2 => true,
        TargetKind.AddTime => true,
        _ => false
    };

    private void DoHitStop(float seconds)
    {
        if (_isHitStopping)
            return;

        StartCoroutine(HitStopRoutine(seconds));
    }

    private IEnumerator HitStopRoutine(float seconds)
    {
        _isHitStopping = true;

        float oldScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(seconds);

        Time.timeScale = oldScale;
        _isHitStopping = false;
    }

    private void DoShake(float duration, float strength)
    {
        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);

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

            CameraTransform.localPosition = _cameraOriginalPos + new Vector3(x, y, 0f);
            yield return null;
        }

        CameraTransform.localPosition = _cameraOriginalPos;
        _shakeRoutine = null;
    }

    public void X2SlowMo()
    {
        if (_slowMoRoutine != null)
            StopCoroutine(_slowMoRoutine);

        _slowMoRoutine = StartCoroutine(SlowMoRoutine(X2SlowMoScale, X2SlowMoSeconds));
    }

    private IEnumerator SlowMoRoutine(float scale, float seconds)
    {
        while (Time.timeScale == 0f)
            yield return null;

        float old = Time.timeScale;
        Time.timeScale = scale;

        yield return new WaitForSecondsRealtime(seconds);

        Time.timeScale = old;
        _slowMoRoutine = null;
    }
}

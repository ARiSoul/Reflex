using System.Collections;
using UnityEngine;

public sealed class VfxPool : MonoBehaviour
{
    [SerializeField] private ParticleSystem _prefab;
    [SerializeField] private int _prewarm = 16;

    private ObjectPool<ParticleSystem> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<ParticleSystem>(_prefab, _prewarm, transform);
    }

    public void PlayAt(Vector3 position, Color tint)
    {
        var ps = _pool.Get();
        ps.transform.position = position;

        // Tint particles
        var main = ps.main;

        var c = tint;
        c.r = Mathf.Clamp01(c.r * 1.35f);
        c.g = Mathf.Clamp01(c.g * 1.35f);
        c.b = Mathf.Clamp01(c.b * 1.35f);
        main.startColor = c;

        ps.gameObject.SetActive(true);
        ps.Play();

        StartCoroutine(ReleaseAfter(ps));
    }

    private IEnumerator ReleaseAfter(ParticleSystem ps)
    {
        // Wait in real time so hitstop doesn't freeze VFX cleanup
        yield return new WaitForSecondsRealtime(0.45f);

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _pool.Release(ps);
    }
}

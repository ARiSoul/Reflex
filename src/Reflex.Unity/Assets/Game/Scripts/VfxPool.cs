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
        main.startColor = tint;

        ps.gameObject.SetActive(true);
        ps.Play();

        StartCoroutine(ReleaseAfter(ps, main.duration + main.startLifetime.constantMax + 0.05f));
    }

    private IEnumerator ReleaseAfter(ParticleSystem ps, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _pool.Release(ps);
    }
}

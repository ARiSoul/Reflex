using UnityEngine;

public sealed class FloatingTextPool : MonoBehaviour
{
    [SerializeField] private FloatingTextView _prefab;
    [SerializeField] private int _prewarm = 24;

    private ObjectPool<FloatingTextView> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<FloatingTextView>(_prefab, _prewarm, transform);
    }

    public void Spawn(string msg, Color color, Vector3 pos)
    {
        var ft = _pool.Get();

        // When it disables itself, we release next frame by polling its active state
        // (simple + reliable without events)
        ft.Play(
            msg,
            color,
            pos + new Vector3(0f, 0.2f, 0f),
            velocity: new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(1.2f, 1.8f), 0f),
            lifetime: 0.55f
        );

        // Release watcher
        StartCoroutine(ReleaseWhenDisabled(ft));
    }

    private System.Collections.IEnumerator ReleaseWhenDisabled(FloatingTextView view)
    {
        // Wait until it turns itself off
        while (view.gameObject.activeSelf)
            yield return null;

        _pool.Release(view);
    }
}

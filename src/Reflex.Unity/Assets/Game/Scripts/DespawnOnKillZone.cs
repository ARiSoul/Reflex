using UnityEngine;

public sealed class DespawnOnKillZone : MonoBehaviour
{
    private TargetView _view;

    private void Awake() => _view = GetComponent<TargetView>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("KillZone"))
            _view.Despawn();
    }
}

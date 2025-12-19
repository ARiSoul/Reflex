using UnityEngine;

public sealed class DespawnOnKillZone : MonoBehaviour
{
    private GateView _view;

    private void Awake() => _view = GetComponent<GateView>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("KillZone"))
            _view.Despawn();
    }
}

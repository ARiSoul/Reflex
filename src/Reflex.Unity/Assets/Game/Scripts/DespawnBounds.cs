using UnityEngine;

public sealed class DespawnBounds : MonoBehaviour
{
    [Header("Refs")]
    public GateSpawner GateSpawner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var gate = other.GetComponent<GateView>();
        if (gate == null)
            return;

        // Important: do NOT disable the object here.
        // This will trigger GateView.OnDespawned -> GateSpawner.HandleDespawned -> pool release + active count decrement.
        GateSpawner.Release(gate);
    }
}

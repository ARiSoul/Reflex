using UnityEngine;

public sealed class InputShooter : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private LayerMask _targetMask;

    public bool TryGetHit(out RaycastHit2D hit)
    {
        hit = default;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = _cam.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(pos, Vector2.zero, 0f, _targetMask);
            return hit.collider != null;
        }

        return false;
    }
}

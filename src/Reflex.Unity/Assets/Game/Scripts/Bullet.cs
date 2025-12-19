using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Life = 3f;

    private void Awake()
    {
        Destroy(gameObject, Life);
    }
}

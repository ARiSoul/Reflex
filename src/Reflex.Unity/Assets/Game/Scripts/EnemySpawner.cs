using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnRate = 20f;

    private float _counter = 0f;

    private void FixedUpdate()
    {
        _counter += Time.fixedDeltaTime;

        if (_counter >= SpawnRate)
        {
            SpawnEnemy();
            _counter = 0f;
        }
    }

    private void SpawnEnemy()
    {
        // get random position in this transform in the x axis
        var randomX = Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);

        Vector3 spawnPosition = new(randomX, transform.position.y, transform.position.z);

        Instantiate(EnemyPrefab, spawnPosition, Quaternion.Euler(180, 0, 0));
    }
}

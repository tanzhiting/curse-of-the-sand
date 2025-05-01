using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject mummyPrefab;
    public GameObject anubisPrefab;
    public GameObject pharaohPrefab;

    public Transform[] spawnPoints;
    public float baseSpawnInterval = 5f;
    public int maxEnemies = 20;
    public int initialSpawnCount = 15;

    private float timer = 0f;

    void Start()
    {
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnEnemy();
        }
    }

    void Update()
    {
        int currentEnemies = CountActiveEnemies();

        float spawnInterval = Mathf.Lerp(1f, baseSpawnInterval, currentEnemies / (float)maxEnemies);

        timer += Time.deltaTime;
        if (timer >= spawnInterval && currentEnemies < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        GameObject prefabToSpawn;
        float r = Random.value;

        if (r < 0.5f)
            prefabToSpawn = mummyPrefab;
        else if (r < 0.8f)
            prefabToSpawn = anubisPrefab;
        else
            prefabToSpawn = pharaohPrefab;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = EnemyPoolManager.Instance.GetEnemyFromPool(prefabToSpawn);
        if (enemy != null)
        {
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = Quaternion.identity;
        }
    }

    int CountActiveEnemies()
    {
        int count = 0;
        foreach (var pool in EnemyPoolManager.Instance.enemyPools)
        {
            foreach (var enemy in pool.pool)
            {
                if (enemy.activeInHierarchy)
                    count++;
            }
        }
        return count;
    }
}
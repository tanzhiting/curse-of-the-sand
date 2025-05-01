using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject mummyPrefab;
    public GameObject anubisPrefab;
    public GameObject pharaohPrefab;

    public float spawnInterval = 5f;
    public Transform[] spawnPoints;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 2f, spawnInterval);
    }

    void SpawnEnemy()
    {
        // 随机选择一个 SpawnPoint
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 随机选择生成的敌人
        float r = Random.value;
        GameObject prefabToSpawn = null;

        if (r < 0.5f)
            prefabToSpawn = mummyPrefab;
        else if (r < 0.8f)
            prefabToSpawn = anubisPrefab;
        else
            prefabToSpawn = pharaohPrefab;

        // 同时在多个 spawnPoint 生成敌人
        Instantiate(prefabToSpawn, point.position, Quaternion.identity, point);
    }
}

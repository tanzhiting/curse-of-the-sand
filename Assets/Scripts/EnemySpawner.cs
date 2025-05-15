using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject mummyPrefab;
    public GameObject anubisPrefab;
    public GameObject pharaohPrefab;

    public Transform[] spawnPoints;
    public float baseSpawnInterval = 30f;
    public int maxEnemies = 50;
    public int initialSpawnCount = 20;

    private float timer = 0f;

    void Start()
    {
        // 吸附 spawnPoints 到最近 NavMesh 地面（偏移不能太远）
        foreach (Transform spawnPoint in spawnPoints)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPoint.position, out hit, 5f, NavMesh.AllAreas))
            {
                float distance = Vector3.Distance(spawnPoint.position, hit.position);
                if (distance < 2f)
                {
                    spawnPoint.position = hit.position;
                }
                else
                {
                    Debug.LogWarning("SpawnPoint 偏移过远！跳过吸附。原始位置: " + spawnPoint.position + " → NavMesh位置: " + hit.position + "，偏移: " + distance);
                }
            }
            else
            {
                Debug.LogWarning("SpawnPoint 无法吸附到 NavMesh！位于：" + spawnPoint.position);
            }
        }

        // 计算每个spawn point应生成的敌人数量
        int enemiesPerSpawnPoint = initialSpawnCount / spawnPoints.Length;
        int remainingEnemies = initialSpawnCount % spawnPoints.Length;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int spawnCount = enemiesPerSpawnPoint + (i < remainingEnemies ? 1 : 0);
            for (int j = 0; j < spawnCount; j++)
            {
                SpawnEnemyAtPoint(spawnPoints[i]);
            }
        }
    }

    void Update()
    {
        int currentEnemies = CountActiveEnemies();
        float spawnInterval = Mathf.Lerp(1f, baseSpawnInterval, currentEnemies / (float)maxEnemies);

        timer += Time.deltaTime;
        if (timer >= spawnInterval && currentEnemies < maxEnemies)
        {
            SpawnEnemy(); // 动态生成使用随机点
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        SpawnEnemyAtPoint(randomPoint);
    }

    void SpawnEnemyAtPoint(Transform spawnPoint)
    {
        GameObject prefabToSpawn;
        float r = Random.value;

        if (r < 0.5f)
            prefabToSpawn = mummyPrefab;
        else if (r < 0.8f)
            prefabToSpawn = anubisPrefab;
        else
            prefabToSpawn = pharaohPrefab;

        const int maxTries = 10;
        for (int i = 0; i < maxTries; i++)
        {
            NavMeshHit hit;
            float sampleRadius = 2f;
            if (NavMesh.SamplePosition(spawnPoint.position, out hit, sampleRadius, NavMesh.AllAreas))
            {
                float offset = Vector3.Distance(hit.position, spawnPoint.position);
                if (offset < 1f)
                {
                    GameObject enemy = EnemyPoolManager.Instance.GetEnemyFromPool(prefabToSpawn);
                    if (enemy != null)
                    {
                        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                        if (agent != null)
                        {
                            agent.Warp(hit.position);
                        }
                        else
                        {
                            enemy.transform.position = hit.position;
                        }
                        enemy.transform.rotation = Quaternion.LookRotation(Vector3.forward); // 或 Quaternion.identity;
                        return;
                    }
                }
            }
        }

        Debug.LogWarning("未找到足够靠近的 NavMesh 位置用于生成敌人！");
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

    // Scene 视图中画出红/绿球表示 spawnPoint 是否在 NavMesh 上
    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        foreach (var point in spawnPoints)
        {
            if (point == null) continue;

            NavMeshHit hit;
            bool valid = NavMesh.SamplePosition(point.position, out hit, 1f, NavMesh.AllAreas);
            float distance = Vector3.Distance(point.position, hit.position);

            Gizmos.color = (valid && distance < 1f) ? Color.green : Color.red;
            Gizmos.DrawSphere(point.position, 0.3f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(point.position + Vector3.up * 0.5f, point.name);
#endif
        }
    }
}
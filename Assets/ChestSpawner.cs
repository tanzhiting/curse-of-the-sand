using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class ChestSpawner : MonoBehaviour
{
    public GameObject chestPrefab;
    public Transform chestContainer;
    public List<Transform> allSpawnPoints;
    public Transform playerTransform;

    public float sampleRange = 2f;      // 检查 NavMesh 范围
    public float desiredY = 31.5f;      // 如果 NavMesh 不工作，备用高度

    private List<Transform> availableSpawners;
    private Dictionary<Transform, GameObject> activeChests = new();

    void Start()
    {
        availableSpawners = new List<Transform>(allSpawnPoints);
        SpawnInitialChests(4);
    }

    void SpawnInitialChests(int count)
    {
        for (int i = 0; i < count && availableSpawners.Count > 0; i++)
        {
            SpawnOneChest();
        }
    }

    void SpawnOneChest()
    {
        int index = Random.Range(0, availableSpawners.Count);
        Transform spawnPoint = availableSpawners[index];

        // 计算朝向玩家的角度
        Vector3 lookDir = playerTransform.position - spawnPoint.position;
        lookDir.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(lookDir);

        // ✅ 使用 NavMesh.SamplePosition 来贴地
        Vector3 samplePos = spawnPoint.position;
        bool usedNavMesh = false;

        if (NavMesh.SamplePosition(spawnPoint.position, out NavMeshHit hit, sampleRange, NavMesh.AllAreas))
        {
            samplePos = hit.position;
            usedNavMesh = true;
        }
        else
        {
            samplePos.y = desiredY;
        }

        GameObject chest = Instantiate(chestPrefab, samplePos, rotation, chestContainer);

        // ✅ Debug 显示贴地方式
        string method = usedNavMesh ? "✅ NavMesh" : "❌ desiredY fallback";
        Debug.Log($"Chest spawned at Y = {samplePos.y:F2} using {method}");

        // ✅ Debug 可视化生成点
        Debug.DrawRay(samplePos, Vector3.up * 2f, usedNavMesh ? Color.green : Color.red, 3f);

        // 注册
        activeChests[spawnPoint] = chest;
        availableSpawners.RemoveAt(index);

        ChestTouch chestScript = chest.GetComponent<ChestTouch>();
        if (chestScript != null)
        {
            chestScript.OnChestDestroyed += () => OnChestDestroyed(spawnPoint);
        }
    }

    void OnChestDestroyed(Transform spawner)
    {
        availableSpawners.Add(spawner);
        activeChests.Remove(spawner);

        if (availableSpawners.Count > 0)
            SpawnOneChest();
    }
}
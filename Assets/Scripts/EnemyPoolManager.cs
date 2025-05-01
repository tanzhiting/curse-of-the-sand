using UnityEngine;
using System.Collections.Generic;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance;

    [System.Serializable]
    public class EnemyPool
    {
        public GameObject prefab;
        public int initialSize = 5;
        public List<GameObject> pool = new List<GameObject>();
    }

    public EnemyPool[] enemyPools;
    public Transform enemyParent;

    void Awake()
    {
        Instance = this;

        foreach (var pool in enemyPools)
        {
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject enemy = Instantiate(pool.prefab, enemyParent);
                enemy.SetActive(false);
                pool.pool.Add(enemy);
            }
        }
    }

    public GameObject GetEnemyFromPool(GameObject prefab)
    {
        foreach (var pool in enemyPools)
        {
            if (pool.prefab == prefab)
            {
                foreach (var enemy in pool.pool)
                {
                    if (!enemy.activeInHierarchy)
                    {
                        // 复用敌人时，重置其状态
                        enemy.SetActive(true);
                        enemy.GetComponent<EnemyPatternHandler>().ResetEnemy();
                        return enemy;
                    }
                }

                // 如果池中没有可复用的敌人，创建一个新的敌人
                GameObject newEnemy = Instantiate(prefab, enemyParent);
                newEnemy.SetActive(false);
                pool.pool.Add(newEnemy);
                return newEnemy;
            }
        }

        return null;
    }
}
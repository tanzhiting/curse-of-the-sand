using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EnemyPatternHandler currentTarget; // 当前目标敌人

    void Start()
    {
        // 在游戏开始时，立即选择第一个敌人
        SelectClosestEnemy();
    }

    void Update()
    {
        // 如果当前目标为空，或目标已经不在场景中，寻找最近的敌人
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            SelectClosestEnemy(); // 寻找并选中最近的敌人
        }
    }

    // 寻找并选中最近的敌人
    private void SelectClosestEnemy()
    {
        EnemyPatternHandler[] enemies = FindObjectsByType<EnemyPatternHandler>(FindObjectsSortMode.None); // 查找所有敌人
        if (enemies.Length == 0) return; // 没有敌人时返回

        float minDist = Mathf.Infinity; // 设置初始最小距离为无限大
        Transform cam = Camera.main.transform; // 获取主相机的 Transform

        foreach (var e in enemies)
        {
            float dist = Vector3.Distance(e.transform.position, cam.position); // 计算和相机的距离
            if (dist < minDist)
            {
                minDist = dist; // 更新最小距离
                currentTarget = e; // 设置当前目标为距离最小的敌人
            }
        }
    }

    // 按钮点击时处理输入
    public void OnButtonClick(string pattern)
    {
        // 如果没有目标敌人，或者目标已经被销毁，返回
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
            return;

        try
        {
            // 尝试将按钮名称转化为对应的 PatternType 枚举
            PatternType inputType = (PatternType)Enum.Parse(typeof(PatternType), pattern);
            currentTarget.ReceiveInput(inputType); // 传递输入到敌人处理方法
        }
        catch (Exception e)
        {
            // 捕捉无效的输入，并打印警告
            Debug.LogWarning($"Invalid input pattern: {pattern}, Error: {e.Message}");
        }
    }
}
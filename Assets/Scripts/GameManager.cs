using System;
using UnityEngine;

/// <summary>
/// 控制游戏核心逻辑：输入、敌人攻击、统计、初始化等
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 5f;
    public Transform player;

    [Header("Player Stats")]
    public PlayerStatsSO playerStats;

    void Start()
    {
        InitGame();
    }

    /// <summary>
    /// 初始化本局游戏：重置玩家击杀和碎片数据
    /// </summary>
    private void InitGame()
    {
        if (playerStats != null)
        {
            playerStats.ResetFragmentCounts();
            Debug.Log("GameManager: 玩家数据已重置");
        }
    }

    /// <summary>
    /// 玩家点击图案按钮时触发，对范围内的敌人发送图案输入
    /// </summary>
    /// <param name="pattern">按钮代表的图案名</param>
    public void OnButtonClick(string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return;

        PatternType inputType = (PatternType)Enum.Parse(typeof(PatternType), pattern);
        EnemyPatternHandler[] enemies = FindObjectsByType<EnemyPatternHandler>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(player.position, enemy.transform.position);
            if (dist <= attackRange)
            {
                enemy.ReceiveInput(inputType);
            }
        }
    }

    /// <summary>
    /// 记录每次击败敌人的行为，同时判断是否是新纪录
    /// </summary>
    public void RecordEnemyKill()
    {
        if (playerStats != null)
        {
            playerStats.enemiesDefeatedThisRun++;

            if (playerStats.enemiesDefeatedThisRun > playerStats.bestEnemiesDefeated)
            {
                playerStats.bestEnemiesDefeated = playerStats.enemiesDefeatedThisRun;
            }

            Debug.Log($"Enemy defeated! Total this run: {playerStats.enemiesDefeatedThisRun}");
        }
    }

    /// <summary>
    /// 在 Scene 中绘制绿色圆圈以显示攻击范围（仅选中对象时可见）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(player.position, attackRange);
        }
    }
}
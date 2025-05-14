using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Game/Player Stats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Enemy Stats")]
    public int enemiesDefeatedThisRun;
    public int bestEnemiesDefeated;

    [Header("Fragment Stats (This Run Only)")]
    public FragmentData[] fragmentTypes; // 设定的所有碎片
    public int[] fragmentCountsThisRun;  // 与 fragmentTypes 对应
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Game/Player Stats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Enemy Stats")]
    public int enemiesDefeatedThisRun;
    public int bestEnemiesDefeated;

    // 重置所有统计数据（在每局游戏开始时调用）
    [Header("Fragment Stats (This Run Only)")]
    public List<FragmentEntry> fragmentCountsThisRun = new();

    public void ResetFragmentCounts()
    {
        enemiesDefeatedThisRun = 0;
        fragmentCountsThisRun.Clear();
    }

    public void AddFragment(FragmentData data, int count)
    {
        if (data == null) return;

        foreach (var entry in fragmentCountsThisRun)
        {
            if (entry.fragment == data)
            {
                entry.count += count;
                return;
            }
        }

        fragmentCountsThisRun.Add(new FragmentEntry { fragment = data, count = count });
    }

    public int GetCount(FragmentData data)
    {
        foreach (var entry in fragmentCountsThisRun)
        {
            if (entry.fragment == data)
                return entry.count;
        }
        return 0;
    }

    public List<FragmentData> GetCollectedFragmentTypesThisRun()
    {
        List<FragmentData> collected = new();
        foreach (var entry in fragmentCountsThisRun)
        {
            if (entry.count > 0 && entry.fragment != null)
            {
                collected.Add(entry.fragment);
            }
        }
        return collected;
    }

    public Dictionary<FragmentData, int> GetAllCollectedFragments()
    {
        Dictionary<FragmentData, int> result = new();
        foreach (var entry in fragmentCountsThisRun)
        {
            if (entry.fragment != null)
            {
                result[entry.fragment] = entry.count;
            }
        }
        return result;
    }
}
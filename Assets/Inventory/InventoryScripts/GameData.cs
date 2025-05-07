using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    public List<FragmentEntry> playerFragments = new List<FragmentEntry>();

    [System.Serializable]
    public class FragmentEntry
    {
        public FragmentData fragment;
        public int count;
    }

    // 辅助函数：查找对应碎片的数量
    public int GetFragmentCount(FragmentData data)
    {
        foreach (var entry in playerFragments)
        {
            if (entry.fragment == data)
                return entry.count;
        }
        return 0;
    }

    public void SetFragmentCount(FragmentData data, int amount)
    {
        foreach (var entry in playerFragments)
        {
            if (entry.fragment == data)
            {
                entry.count = amount;
                return;
            }
        }

        // 如果没有找到，添加新条目
        playerFragments.Add(new FragmentEntry { fragment = data, count = amount });
    }
}
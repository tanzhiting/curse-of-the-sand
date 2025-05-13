using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    [System.Serializable]
    public class FragmentEntry
    {
        public FragmentData fragment;
        public int count;
    }

    // 储存玩家的碎片数据
    public List<FragmentEntry> playerFragments = new List<FragmentEntry>();

    // ✅ 新增：记录已合成宝物（跨场景可用）
    [SerializeField]
    private List<TreasureData> craftedTreasures = new List<TreasureData>();

    // ====== 碎片部分 ======

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

        playerFragments.Add(new FragmentEntry { fragment = data, count = amount });
    }

    // ====== 宝物合成记录部分 ======

    // 检查是否已合成
    public bool HasCraftedTreasure(TreasureData treasure)
    {
        return craftedTreasures.Contains(treasure);
    }

    // 添加到已合成列表（避免重复）
    public void AddCraftedTreasure(TreasureData treasure)
    {
        if (!craftedTreasures.Contains(treasure))
        {
            craftedTreasures.Add(treasure);
        }
    }

    // 清除合成记录（用于测试或重置）
    public void ClearCraftedTreasures()
    {
        craftedTreasures.Clear();
    }

    // 获取所有合成的宝物
    public List<TreasureData> GetCraftedTreasures()
    {
        return craftedTreasures;
    }
}
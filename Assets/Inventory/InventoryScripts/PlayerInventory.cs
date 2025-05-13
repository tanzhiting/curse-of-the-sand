using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Game/PlayerInventory")]
public class PlayerInventorySO : ScriptableObject
{
    public GameData gameData;

    // ======================
    // 碎片相关接口
    // ======================

    // 获取碎片数量
    public int GetFragmentCount(FragmentData fragment)
    {
        return gameData.GetFragmentCount(fragment);
    }

    // 增加碎片数量
    public void AddFragment(FragmentData fragment, int amount)
    {
        int current = gameData.GetFragmentCount(fragment);
        gameData.SetFragmentCount(fragment, current + amount);
    }

    // 设置碎片数量（用于初始化或调试）
    public void SetFragmentCount(FragmentData fragment, int amount)
    {
        gameData.SetFragmentCount(fragment, amount);
    }

    // 是否拥有足够碎片来合成宝物
    public bool HasEnoughFragments(TreasureData treasure)
    {
        foreach (var req in treasure.requiredFragments)
        {
            int owned = GetFragmentCount(req.fragment);
            if (owned < req.requiredAmount)
                return false;
        }
        return true;
    }

    // 使用碎片进行合成，如果不足返回 false
    public bool UseFragments(TreasureData treasure)
    {
        if (!HasEnoughFragments(treasure))
            return false;

        foreach (var req in treasure.requiredFragments)
        {
            int owned = GetFragmentCount(req.fragment);
            gameData.SetFragmentCount(req.fragment, owned - req.requiredAmount);
        }

        return true;
    }

    // ======================
    // 宝物合成记录接口
    // ======================

    // 是否已经合成过某个宝物
    public bool HasCraftedTreasure(TreasureData treasure)
    {
        return gameData.HasCraftedTreasure(treasure);
    }

    // 记录一个已合成的宝物
    public void AddCraftedTreasure(TreasureData treasure)
    {
        gameData.AddCraftedTreasure(treasure);
    }
}
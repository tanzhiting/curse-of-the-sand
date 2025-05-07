using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Game/PlayerInventory")]
public class PlayerInventorySO : ScriptableObject
{
    public GameData gameData;

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
}
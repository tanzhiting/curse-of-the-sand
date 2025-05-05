using UnityEngine;

[CreateAssetMenu(fileName = "New Treasure", menuName = "Inventory/Treasure")]
public class TreasureData : ScriptableObject
{
    public string itemID;
    [TextArea]
    public string fullName;
    public string blurredName;
    [TextArea]
    public string fullDescription;
    [TextArea]
    public string blurredDescription;
    public Sprite image;

    [System.Serializable]
    public class FragmentRequirement
    {
        public FragmentData fragment;  // 引用碎片
        public int requiredAmount;
    }

    public FragmentRequirement[] requiredFragments;

    // 获取当前碎片进度
    public string GetName(int collectedFragments)
    {
        if (collectedFragments >= GetRequiredAmount() / 2) 
        {
            // 显示模糊描述
            return blurredName == "???" ? fullName : blurredName;
        }
        return "???"; // 初始状态时名字为???
    }

    public string GetDescription(int collectedFragments)
    {
        if (collectedFragments >= GetRequiredAmount()) 
        {
            return fullDescription;  // 完整描述
        }
        else if (collectedFragments >= GetRequiredAmount() / 2)
        {
            return blurredDescription;  // 模糊描述
        }
        return "???"; // 初始状态时描述为???
    }

    // 获取当前宝物的碎片需求总数
    private int GetRequiredAmount()
    {
        int totalAmount = 0;
        foreach (var fragment in requiredFragments)
        {
            totalAmount += fragment.requiredAmount;
        }
        return totalAmount;
    }
}
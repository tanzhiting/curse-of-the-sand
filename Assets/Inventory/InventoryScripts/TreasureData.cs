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
        public FragmentData fragment;
        public int requiredAmount;
    }

    public FragmentRequirement[] requiredFragments;

    // ✅ 提供一个通用的需求总数，供外部使用
    public int GetRequiredAmount()
    {
        int total = 0;
        foreach (var req in requiredFragments)
        {
            total += req.requiredAmount;
        }
        return total;
    }
}
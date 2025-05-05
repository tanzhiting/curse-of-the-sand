using UnityEngine;
using System.Collections.Generic;

public class BackpackManager : MonoBehaviour
{
    public List<TreasureData> allTreasures;
    public GameObject itemSlotPrefab;
    public Transform itemGrid;
    public ItemDetailPanel detailPanel;

    // 假设玩家拥有碎片数量的数据库（用字典存储）
    public Dictionary<FragmentData, int> playerFragmentCounts;

    void Start()
    {
        foreach (var treasure in allTreasures)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, itemGrid);
            var slot = slotObj.GetComponent<CraftItemSlot>();

            int[] owned = GetOwnedFragmentCounts(treasure);
            slot.Setup(treasure, this, owned);
        }

        if (allTreasures.Count > 0)
            detailPanel.ShowItem(allTreasures[0], GetOwnedFragmentCounts(allTreasures[0]));
    }

    public void OnItemClicked(TreasureData treasure)
    {
        detailPanel.ShowItem(treasure, GetOwnedFragmentCounts(treasure));
    }

    private int[] GetOwnedFragmentCounts(TreasureData data)
    {
        int[] result = new int[data.requiredFragments.Length];
        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            var frag = data.requiredFragments[i].fragment;
            result[i] = playerFragmentCounts.ContainsKey(frag) ? playerFragmentCounts[frag] : 0;
        }
        return result;
    }
}
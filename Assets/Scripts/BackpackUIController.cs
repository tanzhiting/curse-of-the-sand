using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BackpackUIController : MonoBehaviour 
{
    public PlayerInventorySO playerInventory;   // 存储玩家背包信息
    public TreasureData[] treasures;           // 背包中的所有宝藏数据
    public Transform itemGrid;                 // 放置物品槽的容器
    public GameObject itemSlotPrefab;          // 物品槽预制体
    public ItemDetailPanel detailPanel;        // 物品详情面板

    private CraftItemSlot currentSelectedSlot; // 当前选中的物品槽

    public BackpackManager backpackManager;    // 背包管理器

    void Start()
    {
        RefreshGrid(); // 初始化背包网格
    }

    // 刷新背包网格（每次打开背包时调用）
    public void RefreshGrid()
    {
        // 清空现有的所有物品槽
        foreach (Transform child in itemGrid)
            Destroy(child.gameObject);

        // 为每个宝藏创建一个物品槽
        foreach (var treasure in treasures)
        {
            var slotObj = Instantiate(itemSlotPrefab, itemGrid); // 实例化物品槽
            var slot = slotObj.GetComponent<CraftItemSlot>();    // 获取物品槽组件
            int[] counts = GetFragmentCounts(treasure);          // 获取碎片数量
            slot.Setup(treasure, backpackManager, counts);       // 传入背包管理器，而不是当前类

            // 订阅点击事件
            slot.OnItemClickedEvent += OnItemClicked;  // 订阅点击事件
        }

        // ✅ 默认选中第一个物品槽并高亮显示
        if (itemGrid.childCount > 0)
        {
            var firstSlot = itemGrid.GetChild(0).GetComponent<CraftItemSlot>(); // 获取第一个物品槽
            currentSelectedSlot = firstSlot;  // 设置为当前选中的槽
            firstSlot.SetSelected(true);  // 设置选中状态
            detailPanel.ShowItem(firstSlot.GetTreasureData(), GetFragmentCounts(firstSlot.GetTreasureData())); // 更新详情面板
        }
    }

    // 物品槽点击事件
    public void OnItemClicked(TreasureData data)
    {
        // 取消之前选中的物品槽
        if (currentSelectedSlot != null)
            currentSelectedSlot.SetSelected(false);

        // 找到新点击的物品槽并选中
        foreach (Transform child in itemGrid)
        {
            var slot = child.GetComponent<CraftItemSlot>();
            if (slot.GetTreasureData() == data) // 根据宝藏数据匹配
            {
                currentSelectedSlot = slot;
                slot.SetSelected(true);
            }
        }

        // 更新详情面板显示
        detailPanel.ShowItem(data, GetFragmentCounts(data));
    }

    // 获取宝藏碎片的数量
    private int[] GetFragmentCounts(TreasureData data)
    {
        int[] result = new int[data.requiredFragments.Length];  // 根据需要的碎片数量初始化数组
        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            var frag = data.requiredFragments[i].fragment;  // 获取每个碎片的类型
            result[i] = playerInventory.GetFragmentCount(frag);  // 获取玩家拥有的碎片数量
        }
        return result;
    }
}
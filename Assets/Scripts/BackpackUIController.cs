using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum BackpackMode
{
    ViewFragmentsOnly,
    Crafting
}

public class BackpackUIController : MonoBehaviour 
{
    public PlayerInventorySO playerInventory;
    public TreasureData[] treasures;
    public Transform itemGrid;
    public GameObject itemSlotPrefab;
    public ItemDetailPanel detailPanel;
    public BackpackManager backpackManager;
    public GameData gameData;

    public BackpackMode mode = BackpackMode.Crafting;

    private CraftItemSlot currentSelectedSlot;

    void Start()
    {
        RefreshGrid();
    }

    public void RefreshGrid()
    {
        foreach (Transform child in itemGrid)
            Destroy(child.gameObject);

        foreach (var treasure in treasures)
        {
            var slotObj = Instantiate(itemSlotPrefab, itemGrid);
            var slot = slotObj.GetComponent<CraftItemSlot>();
            int[] counts = GetFragmentCounts(treasure);
            slot.Setup(treasure, backpackManager, counts, gameData);
            slot.OnItemClickedEvent += OnItemClicked;
        }

        if (itemGrid.childCount > 0)
        {
            var firstSlot = itemGrid.GetChild(0).GetComponent<CraftItemSlot>();
            currentSelectedSlot = firstSlot;
            firstSlot.SetSelected(true);
            UpdateDetailPanel(firstSlot.GetTreasureData());
        }
    }

    public void OnItemClicked(TreasureData data)
    {
        if (currentSelectedSlot != null)
            currentSelectedSlot.SetSelected(false);

        foreach (Transform child in itemGrid)
        {
            var slot = child.GetComponent<CraftItemSlot>();
            if (slot.GetTreasureData() == data)
            {
                currentSelectedSlot = slot;
                slot.SetSelected(true);
                break;
            }
        }

        UpdateDetailPanel(data);
    }

    private int[] GetFragmentCounts(TreasureData data)
    {
        int[] result = new int[data.requiredFragments.Length];
        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            var frag = data.requiredFragments[i].fragment;
            result[i] = playerInventory.GetFragmentCount(frag);
        }
        return result;
    }

    public void UpdateDetailPanel(TreasureData data)
    {
        int[] counts = GetFragmentCounts(data);
        int collected = 0;
        int total = 0;

        string[] fragTextArray = new string[data.requiredFragments.Length];
        Sprite[] fragIconArray = new Sprite[data.requiredFragments.Length];

        bool alreadyCrafted = gameData.HasCraftedTreasure(data);

        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            int required = data.requiredFragments[i].requiredAmount;
            int owned = alreadyCrafted ? required : counts[i];

            collected += Mathf.Min(owned, required);
            total += required;

            string color = owned < required ? "AF1D1D" : "FFFFFF";
            fragTextArray[i] = $"<color=#{color}>{owned}</color>/{required}";
            fragIconArray[i] = data.requiredFragments[i].fragment.icon;
        }

        // ✅ 名称与描述处理，只在 crafted 时显示完整
        string displayName = GetDisplayName(data, collected, alreadyCrafted);
        string displayDesc = GetDisplayDescription(data, collected, alreadyCrafted);

        // ✅ 图片亮度最多到 80%，只有 crafted 时变全白
        float progress = Mathf.Clamp01((float)collected / total);
        float brightness = alreadyCrafted ? 1f : Mathf.Min(progress, 0.5f);
        Color imageColor = Color.Lerp(new Color(0.1f, 0.1f, 0.1f), Color.white, brightness);

        detailPanel.ShowItem(
            displayName,
            displayDesc,
            data.image,
            imageColor,
            fragIconArray,
            fragTextArray
        );

        // ✅ 合成按钮状态处理
        bool isUnlocked = collected >= total;

        if (mode == BackpackMode.Crafting && !alreadyCrafted)
        {
            detailPanel.craftButton.gameObject.SetActive(true);
            detailPanel.craftButton.interactable = isUnlocked;

            if (detailPanel.craftButtonImage != null)
            {
                detailPanel.craftButtonImage.sprite = isUnlocked
                    ? detailPanel.craftAvailableSprite
                    : detailPanel.craftLockedSprite;
            }

            detailPanel.craftButton.onClick.RemoveAllListeners();
            if (isUnlocked)
            {
                detailPanel.craftButton.onClick.AddListener(() => OnCraftButtonClicked(data));
            }
        }
        else
        {
            detailPanel.craftButton.gameObject.SetActive(false);
        }
    }

    private string GetDisplayName(TreasureData data, int collected, bool crafted)
    {
        if (crafted)
            return data.fullName;
        else if (collected >= data.GetRequiredAmount() / 2)
            return data.blurredName;
        else
            return "???";
    }

    private string GetDisplayDescription(TreasureData data, int collected, bool crafted)
    {
        if (crafted)
            return data.fullDescription;
        else if (collected >= data.GetRequiredAmount() / 2)
            return data.blurredDescription;
        else
            return "???";
    }

    private void OnCraftButtonClicked(TreasureData data)
    {
        if (playerInventory.UseFragments(data))
        {
            gameData.AddCraftedTreasure(data);

            // ✅ 刷新显示为完整信息
            UpdateDetailPanel(data);

            // ✅ 刷新整个格子 UI
            RefreshGrid();
        }
    }
}
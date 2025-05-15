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
    // ✅ Singleton 实现
    public static BackpackUIController Instance { get; private set; }

    public PlayerInventorySO playerInventory;
    public TreasureData[] treasures;
    public Transform itemGrid;
    public GameObject itemSlotPrefab;
    public ItemDetailPanel detailPanel;
    public BackpackManager backpackManager;
    public GameData gameData;

    public BackpackMode mode = BackpackMode.Crafting;

    private CraftItemSlot currentSelectedSlot;

    // ✅ 动态绑定 itemGrid + Singleton 初始化
    void Awake()
    {
        // Singleton 初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 自动查找 itemGrid
        if (itemGrid == null)
        {
            Transform canvas = GameObject.Find("Canvas")?.transform;
            Debug.Log("Canvas found: " + (canvas != null));

            Transform backpack = canvas?.Find("Backpack");
            Debug.Log("Backpack found: " + (backpack != null));

            itemGrid = backpack?.Find("ItemGrid");
            Debug.Log("ItemGrid found: " + (itemGrid != null));

            if (itemGrid == null)
                Debug.LogError("BackpackUIController: itemGrid is not assigned and could not be found at Canvas > Backpack > ItemGrid");
        }
    }

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

        string displayName = GetDisplayName(data, collected, alreadyCrafted);
        string displayDesc = GetDisplayDescription(data, collected, alreadyCrafted);

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

            UpdateDetailPanel(data);

            RefreshGrid();
        }
    }
}
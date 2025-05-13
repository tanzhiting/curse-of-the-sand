using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class BackpackManager : MonoBehaviour
{
    public List<TreasureData> allTreasures;
    public GameObject itemSlotPrefab;
    public Transform itemGrid;
    public ItemDetailPanel detailPanel;

    public PlayerInventorySO playerInventory;

    [Header("UI References")]
    public GameObject backpackUI;
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;

    private CraftItemSlot currentSelectedSlot;
    private readonly List<CraftItemSlot> activeSlots = new();

    public static BackpackManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        RefreshAllSlots();
    }

    public void RefreshAllSlots()
    {
        foreach (Transform child in itemGrid)
            Destroy(child.gameObject);

        activeSlots.Clear();

        foreach (var treasure in allTreasures)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, itemGrid);
            var slot = slotObj.GetComponent<CraftItemSlot>();
            int[] owned = GetOwnedFragmentCounts(treasure);
            slot.Setup(treasure, this, owned);

            // 订阅事件
            slot.OnItemClickedEvent += OnItemClicked; 
            activeSlots.Add(slot);
        }

        if (activeSlots.Count > 0)
        {
            currentSelectedSlot = activeSlots[0];
            currentSelectedSlot.SetSelected(true);
            detailPanel.ShowItem(currentSelectedSlot.GetTreasureData(), GetOwnedFragmentCounts(currentSelectedSlot.GetTreasureData()));
        }
    }

    public void OnItemClicked(TreasureData treasure)
    {
        if (currentSelectedSlot != null)
            currentSelectedSlot.SetSelected(false);

        foreach (var slot in activeSlots)
        {
            if (slot.GetTreasureData() == treasure)
            {
                currentSelectedSlot = slot;
                currentSelectedSlot.SetSelected(true);
                break;
            }
        }

        detailPanel.ShowItem(treasure, GetOwnedFragmentCounts(treasure));
    }

    private int[] GetOwnedFragmentCounts(TreasureData data)
    {
        int[] result = new int[data.requiredFragments.Length];
        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            var frag = data.requiredFragments[i].fragment;
            result[i] = playerInventory.GetFragmentCount(frag);
        }
        return result;
    }

    public void OpenBackpack()
    {
        backpackUI.SetActive(true);
        RefreshAllSlots();
        Time.timeScale = 0f;
    }

    public void CloseBackpack()
    {
        backpackUI.SetActive(false);
        StartCoroutine(ResumeGameCountdown());
    }

    private IEnumerator ResumeGameCountdown()
    {
        countdownPanel.SetActive(true);
        countdownText.gameObject.SetActive(true);

        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            countdownText.transform.localScale = Vector3.one * 1.5f;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * 5f;
                float scale = Mathf.Lerp(1.5f, 1f, t);
                countdownText.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        countdownPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void NotifyFragmentGained()
    {
        RefreshAllSlots(); 
    }
}
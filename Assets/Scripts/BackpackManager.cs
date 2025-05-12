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

    public PlayerInventorySO playerInventory; // ← 新增引用 ScriptableObject

    [Header("UI References")]
    public GameObject backpackUI;                // 背包主界面（整体父对象）
    public GameObject countdownPanel;            // 倒数面板（带遮罩）
    public TextMeshProUGUI countdownText;        // 倒数字体（挂在 Panel 下）

    private CraftItemSlot currentSelectedSlot;

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
        // 1. 找到点击的那个 slot
        CraftItemSlot clickedSlot = null;
        foreach (Transform child in itemGrid)
        {
            var slot = child.GetComponent<CraftItemSlot>();
            if (slot != null && slot.GetTreasureData() == treasure)
            {
                clickedSlot = slot;
                break;
            }
        }

        // 2. 取消之前的高亮
        if (currentSelectedSlot != null)
            currentSelectedSlot.SetSelected(false);

        // 3. 设置新的高亮
        if (clickedSlot != null)
        {
            currentSelectedSlot = clickedSlot;
            currentSelectedSlot.SetSelected(true);
        }

        detailPanel.ShowItem(treasure, GetOwnedFragmentCounts(treasure));
    }

    private int[] GetOwnedFragmentCounts(TreasureData data)
    {
        int[] result = new int[data.requiredFragments.Length];
        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            var frag = data.requiredFragments[i].fragment;
            result[i] = playerInventory.GetFragmentCount(frag); // ← 用 ScriptableObject 获取
        }
        return result;
    }

    // 打开背包，暂停游戏
    public void OpenBackpack()
    {
        backpackUI.SetActive(true);
        Time.timeScale = 0f;
    }

    // 关闭背包，触发倒数恢复游戏
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

            // 简单放大动画
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
}
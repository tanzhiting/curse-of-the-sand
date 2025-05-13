using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class BackpackManager : MonoBehaviour
{
    public GameObject backpackUI;
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;

    public static BackpackManager Instance;

    public Action OnBackpackOpened;      // 通知 UI Controller 刷新 Craft 列表
    public Action OnFragmentsUpdated;    // 通知碎片数量更新（刷新格子 UI）

    [Header("Craft Detection")]
    public PlayerInventorySO inventory;  // 玩家碎片仓库
    public TreasureData[] allTreasures;  // 所有宝物定义
    public GameData gameData;            // 存档记录（是否已合成）
    public UIManager uiManager;          // 控制红点图标
    public NotificationUI notificationUI; // 弹出合成提示

    private bool hasPendingCraftableTreasure = false;

    // Backpack UI Controller Reference
    public BackpackUIController backpackUIController; // ✅ 新增的引用

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 打开背包（暂停游戏，移除红点提示）
    /// </summary>
    public void OpenBackpack()
    {
        backpackUI.SetActive(true);
        Time.timeScale = 0f;

        // ✅ 移除红点提示
        if (hasPendingCraftableTreasure)
        {
            hasPendingCraftableTreasure = false;
            uiManager.SetBackpackHasNewCraft(false); // 隐藏红点图标
        }

        OnBackpackOpened?.Invoke(); // 通知 UI 刷新
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

    /// <summary>
    /// 外部调用：新增碎片后，通知背包刷新 & 检查可合成状态
    /// </summary>
    public void NotifyFragmentGained()
    {
        OnFragmentsUpdated?.Invoke();
        CheckForCraftableTreasureAfterCollect();
    }

    /// <summary>
    /// 检查是否可以合成任意宝物，如果是，则弹出提示并激活红点图标。
    /// </summary>
    public TreasureData CheckForCraftableTreasureAfterCollect()
    {
        foreach (var treasure in allTreasures)
        {
            if (gameData.HasCraftedTreasure(treasure)) continue;

            bool canCraft = true;
            foreach (var fragInfo in treasure.requiredFragments)
            {
                int owned = inventory.GetFragmentCount(fragInfo.fragment);
                if (owned < fragInfo.requiredAmount)
                {
                    canCraft = false;
                    break;
                }
            }

            if (canCraft)
            {
                if (!hasPendingCraftableTreasure)
                {
                    hasPendingCraftableTreasure = true;
                    uiManager.SetBackpackHasNewCraft(true); // 红点
                }

                return treasure; // ✅ 返回可合成的宝物
            }
        }

        return null; // ❌ 没有可合成的宝物
    }

    /// <summary>
    /// 刷新背包 UI，更新所有碎片信息
    /// </summary>
    public void RefreshGrid()
    {
        backpackUIController.RefreshGrid(); // 调用 BackpackUIController 刷新背包格子 UI
    }
}
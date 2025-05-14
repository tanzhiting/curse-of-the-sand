using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameOverPanel : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI bestAmountText;
    public Transform fragmentGridParent; // 所有碎片UI prefab的容器
    public GameObject fragmentDisplayPrefab;

    [Header("Data")]
    public PlayerStatsSO playerStats;

    void OnEnable()
    {
        // 显示敌人数
        amountText.text = $"{playerStats.enemiesDefeatedThisRun}";

        // 更新并显示最佳敌人击败数
        if (playerStats.enemiesDefeatedThisRun > playerStats.bestEnemiesDefeated)
        {
            playerStats.bestEnemiesDefeated = playerStats.enemiesDefeatedThisRun;
        }
        bestAmountText.text = $"{playerStats.bestEnemiesDefeated}";

        // 清空旧UI
        foreach (Transform child in fragmentGridParent)
        {
            Destroy(child.gameObject);
        }

        // 获取所有本局收集到的碎片类型和数量
        List<FragmentEntry> collectedFragments = playerStats.fragmentCountsThisRun;

        bool hasValidFragments = false;

        foreach (var fragmentEntry in collectedFragments)
        {
            if (fragmentEntry.count > 0 && fragmentEntry.fragment != null)
            {
                GameObject go = Instantiate(fragmentDisplayPrefab, fragmentGridParent);
                FragmentDisplayUI displayUI = go.GetComponent<FragmentDisplayUI>();
                displayUI.Setup(fragmentEntry.fragment.icon, fragmentEntry.count);
                hasValidFragments = true;
            }
        }

        // 根据是否有有效碎片决定是否显示 fragmentGridParent
        fragmentGridParent.gameObject.SetActive(hasValidFragments);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        // 显示每种碎片
        for (int i = 0; i < playerStats.fragmentTypes.Length; i++)
        {
            if (playerStats.fragmentCountsThisRun[i] > 0)
            {
                GameObject go = Instantiate(fragmentDisplayPrefab, fragmentGridParent);
                go.transform.GetChild(0).GetComponent<Image>().sprite = playerStats.fragmentTypes[i].icon;
                go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerStats.fragmentCountsThisRun[i].ToString();
            }
        }
    }
}
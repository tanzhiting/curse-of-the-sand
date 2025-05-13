using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftItemSlot : MonoBehaviour
{
    public Image mainImage;
    public Transform fragmentGroup;              // 父物体（用于排列碎片图标）
    public GameObject fragmentSlotPrefab;        // 小图标+数量的预制体

    public Image backgroundImage;                // 背景图（用于高亮）
    public Sprite normalBg;
    public Sprite selectedBg;

    private TreasureData treasureData;
    private BackpackManager manager;

    public delegate void ItemSlotClicked(TreasureData data); // 定义点击事件委托
    public event ItemSlotClicked OnItemClickedEvent;

    /// <summary>
    /// 初始化设置 CraftItemSlot 的显示。
    /// </summary>
    public void Setup(TreasureData data, BackpackManager manager, int[] ownedCounts, GameData gameData)
    {
        this.treasureData = data;
        this.manager = manager;

        mainImage.sprite = data.image;
        SetPreserveAspect(mainImage);

        // 清空旧的碎片UI
        foreach (Transform child in fragmentGroup)
            Destroy(child.gameObject);

        int collected = 0;
        int total = 0;

        bool alreadyCrafted = gameData.HasCraftedTreasure(data); // ✅ 判断是否已合成

        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            var frag = data.requiredFragments[i];
            int required = frag.requiredAmount;
            int owned = alreadyCrafted ? required : ownedCounts[i]; // ✅ 若已合成，则视为拥有所有

            collected += Mathf.Min(owned, required);
            total += required;

            // 创建碎片槽 UI
            GameObject fragSlot = Instantiate(fragmentSlotPrefab, fragmentGroup);
            Image icon = fragSlot.transform.Find("FragmentIcon").GetComponent<Image>();
            TMP_Text count = fragSlot.transform.Find("FragmentCount").GetComponent<TMP_Text>();

            icon.sprite = frag.fragment.icon;
            SetPreserveAspect(icon);

            // ✅ 若已合成或拥有足够，文字为白色；否则为红色
            string colorHex = (alreadyCrafted || owned >= required) ? "FFFFFF" : "AF1D1D";
            count.text = $"<color=#{colorHex}>{owned}</color>/{required}";
        }

        // ✅ 合成后显示为白色；否则根据收集进度渐变
        if (alreadyCrafted)
        {
            mainImage.color = Color.white;
        }
        else
        {
            SetRevealProgress(mainImage, collected, total);
        }
    }

    private void SetPreserveAspect(Image image)
    {
        image.preserveAspect = true;
    }

    private void SetRevealProgress(Image image, int collected, int total)
    {
        float progress = Mathf.Clamp01((float)collected / total);
        Color revealColor = Color.Lerp(new Color(0.1f, 0.1f, 0.1f), Color.white, progress); // 暗→亮
        image.color = revealColor;
    }

    // 点击事件绑定在 Button 上时调用
    public void OnClick()
    {
        OnItemClickedEvent?.Invoke(treasureData);
    }

    public TreasureData GetTreasureData()
    {
        return treasureData;
    }

    public void SetSelected(bool isSelected)
    {
        backgroundImage.sprite = isSelected ? selectedBg : normalBg;
    }
}
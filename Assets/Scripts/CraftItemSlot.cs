using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftItemSlot : MonoBehaviour
{
    public Image mainImage;
    public Transform fragmentGroup; // 父物体（用 HorizontalLayoutGroup 排列）
    public GameObject fragmentSlotPrefab; // 小图标+数量的 prefab

    private TreasureData treasureData;
    private BackpackManager manager;

    public Image backgroundImage; // 拖入背景图的引用
    public Sprite normalBg;
    public Sprite selectedBg;

    public void Setup(TreasureData data, BackpackManager manager, int[] ownedCounts)
    {
        this.treasureData = data;
        this.manager = manager;

        mainImage.sprite = data.image;
        SetPreserveAspect(mainImage); // ✅ 保持主图纵横比

        // 清空旧的碎片UI
        foreach (Transform child in fragmentGroup)
            Destroy(child.gameObject);

        int collected = 0;
        int total = 0;
        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            var frag = data.requiredFragments[i];
            collected += Mathf.Min(ownedCounts[i], frag.requiredAmount);
            total += frag.requiredAmount;

            GameObject fragSlot = Instantiate(fragmentSlotPrefab, fragmentGroup);
            Image icon = fragSlot.transform.Find("FragmentIcon").GetComponent<Image>();
            TMP_Text count = fragSlot.transform.Find("FragmentCount").GetComponent<TMP_Text>();

            icon.sprite = frag.fragment.icon;
            SetPreserveAspect(icon); // ✅ 保持图标纵横比

            string ownedStr = $"<color=#{(ownedCounts[i] < frag.requiredAmount ? "AF1D1D" : "FFFFFF")}>{ownedCounts[i]}</color>";
            count.text = $"{ownedStr}/{frag.requiredAmount}";
        }

        // 设置渐变效果
        SetRevealProgress(mainImage, collected, total);
    }

    // ✅ 启用 Preserve Aspect
    private void SetPreserveAspect(Image image)
    {
        image.preserveAspect = true;
    }

    // 设置渐变效果（黑色到白色）
    private void SetRevealProgress(Image image, int collected, int total)
    {
        // 根据收集的碎片数量计算透明度比例
        float progress = Mathf.Clamp01((float)collected / total);

        // 使用 Lerp 淡出黑色并显示图像
        Color targetColor = Color.Lerp(Color.black, Color.white, progress);
        image.color = targetColor;
    }

    // 点击时被 Unity Button 事件调用
    public void OnClick()
    {
        manager.OnItemClicked(treasureData);
    }

    // ✅ 用于 BackpackManager 获取数据
    public TreasureData GetTreasureData()
    {
        return treasureData;
    }

    // ✅ 控制是否选中
    public void SetSelected(bool isSelected)
    {
        backgroundImage.sprite = isSelected ? selectedBg : normalBg;
    }
}
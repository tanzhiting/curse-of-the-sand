using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailPanel : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Image mainImage;
    public Image[] fragmentIcons;
    public TMP_Text[] fragmentTexts;

    public void ShowItem(TreasureData data, int[] ownedCounts)
    {
        // 计算玩家收集的碎片数量和总碎片需求量
        int collected = 0;
        int total = 0;
        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            collected += Mathf.Min(ownedCounts[i], data.requiredFragments[i].requiredAmount);
            total += data.requiredFragments[i].requiredAmount;
        }

        // 更新标题、描述和主图
        titleText.text = data.GetName(collected);
        descriptionText.text = data.GetDescription(collected);
        mainImage.sprite = data.image;
        SetPreserveAspect(mainImage); // 设置主图像保持比例

        // 设置渐变效果
        SetRevealProgress(mainImage, collected, total);

        // 设置碎片图标和数量
        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            fragmentIcons[i].sprite = data.requiredFragments[i].fragment.icon;
            SetPreserveAspect(fragmentIcons[i]); // 设置碎片图标保持比例
            
            // 设置文本并根据条件更改颜色
            string ownedStr = $"<color=#{(ownedCounts[i] < data.requiredFragments[i].requiredAmount ? "AF1D1D" : "FFFFFF")}>{ownedCounts[i]}</color>";
            fragmentTexts[i].text = $"{ownedStr}/{data.requiredFragments[i].requiredAmount}";
        }
    }

    // 设置保持比例
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
}
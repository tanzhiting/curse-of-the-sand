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
        int collected = 0;
        for (int i = 0; i < data.requiredFragments.Length; i++)
            collected += Mathf.Min(ownedCounts[i], data.requiredFragments[i].requiredAmount);

        titleText.text = data.GetName(collected);
        descriptionText.text = data.GetDescription(collected);
        mainImage.sprite = data.image;

        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            fragmentIcons[i].sprite = data.requiredFragments[i].fragment.icon;
            fragmentTexts[i].text = $"{ownedCounts[i]}/{data.requiredFragments[i].requiredAmount}";
        }
    }
}
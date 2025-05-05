using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftItemSlot : MonoBehaviour
{
    public Image itemImage;
    public Image[] fragmentIcons;
    public TMP_Text[] fragmentTexts;

    private TreasureData treasure;
    private BackpackManager manager;

    public void Setup(TreasureData data, BackpackManager manager, int[] ownedCounts)
    {
        this.treasure = data;
        this.manager = manager;

        itemImage.sprite = data.image;

        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            fragmentIcons[i].sprite = data.requiredFragments[i].fragment.icon;
            fragmentTexts[i].text = $"{ownedCounts[i]}/{data.requiredFragments[i].requiredAmount}";
        }
    }

    public void OnClick()
    {
        manager.OnItemClicked(treasure);
    }
}

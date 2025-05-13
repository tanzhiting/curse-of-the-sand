using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailPanel : MonoBehaviour
{
    [Header("Main Info")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image itemImage;

    [Header("Fragment Info")]
    public Image[] fragmentIcons;
    public TMP_Text[] fragmentTexts;

    [Header("Craft Button")]
    public Button craftButton;
    public Image craftButtonImage;
    public Sprite craftAvailableSprite;
    public Sprite craftLockedSprite;

    /// <summary>
    /// 外部传入所有展示数据（已预处理），面板仅做展示
    /// </summary>
    public void ShowItem(
        string title,
        string description,
        Sprite mainSprite,
        Color imageColor,
        Sprite[] fragmentSprites,
        string[] fragmentTextStrings
    )
    {
        nameText.text = title;
        descriptionText.text = description;
        itemImage.sprite = mainSprite;
        itemImage.color = imageColor;
        SetPreserveAspect(itemImage);
        SetPreserveAspect(craftButtonImage);

        for (int i = 0; i < fragmentSprites.Length; i++)
        {
            fragmentIcons[i].sprite = fragmentSprites[i];
            SetPreserveAspect(fragmentIcons[i]);
            fragmentTexts[i].text = fragmentTextStrings[i];
        }

        // 如果碎片数量少于槽位总数，隐藏多余部分
        for (int i = fragmentSprites.Length; i < fragmentIcons.Length; i++)
        {
            fragmentIcons[i].gameObject.SetActive(false);
            fragmentTexts[i].gameObject.SetActive(false);
        }
    }

    private void SetPreserveAspect(Image image)
    {
        image.preserveAspect = true;
    }
}
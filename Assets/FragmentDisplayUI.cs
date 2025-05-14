using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FragmentDisplayUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI countText;

    // 这个方法用于设置保持宽高比
    private void SetPreserveAspect(Image image)
    {
        image.preserveAspect = true;
    }

    public void Setup(Sprite icon, int count)
    {
        iconImage.sprite = icon;
        countText.text = count.ToString();
        
        // 确保图像保持宽高比
        SetPreserveAspect(iconImage);
    }
}
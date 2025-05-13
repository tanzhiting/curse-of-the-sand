using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    public GameObject fragmentNotification; // Fragment(images)
    public GameObject unlockNotification;   // Unlock(images)
    public Image fragmentImage; // 这是指你Fragment通知里的那个Image
    public Image unlockImage; // 这是指你Fragment通知里的那个Image
    public float duration = 2f;

    // 设定图片为保持宽高比
    private void SetPreserveAspect(Image image)
    {
        image.preserveAspect = true;
    }

    public void ShowFragmentNotification(Sprite sprite)
    {
        fragmentNotification.SetActive(true);
        unlockNotification.SetActive(false);

        if (fragmentImage != null && sprite != null)
        {
            fragmentImage.sprite = sprite;

            // 保持宽高比
            SetPreserveAspect(fragmentImage);

            // 亮度逻辑同步 Backpack 的风格
            float brightness = 0.5f;  // 你可以根据需求调整这个值
            // 设置颜色
            Color color = Color.Lerp(new Color(0.1f, 0.1f, 0.1f), Color.white, brightness);
            unlockImage.color = color;
        }

        ShowAndAutoHide();
    }

    public void ShowUnlockNotification(Sprite sprite)
    {
        fragmentNotification.SetActive(false);
        unlockNotification.SetActive(true);
        
        if (unlockImage != null && sprite != null)
        {
            unlockImage.sprite = sprite;

            // 保持宽高比
            SetPreserveAspect(unlockImage);
        }
        
        ShowAndAutoHide();
    }

    void ShowAndAutoHide()
    {
        gameObject.SetActive(true);
        CancelInvoke();
        Invoke(nameof(Hide), duration);
    }

    void Hide()
    {
        gameObject.SetActive(false); // 不 Destroy，场景里有
    }
}
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    public GameObject fragmentNotification; // Fragment(images)
    public GameObject unlockNotification;   // Unlock(images)
    public Image fragmentImage; // ← 这是指你Fragment通知里的那个Image
    public float duration = 2f;

    public void ShowFragmentNotification(Sprite sprite)
    {
        fragmentNotification.SetActive(true);
        unlockNotification.SetActive(false);

        if (fragmentImage != null && sprite != null)
        {
            fragmentImage.sprite = sprite;

            float aspect = (float)sprite.texture.width / sprite.texture.height;
            RectTransform rt = fragmentImage.GetComponent<RectTransform>();
            float height = rt.sizeDelta.y; // 假设高度固定
            rt.sizeDelta = new Vector2(height * aspect, height);
        }

        ShowAndAutoHide();
    }

    public void ShowUnlockNotification()
    {
        fragmentNotification.SetActive(false);
        unlockNotification.SetActive(true);
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
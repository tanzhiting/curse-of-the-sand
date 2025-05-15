using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    public static NotificationUI Instance { get; private set; }

    public GameObject fragmentNotification;
    public GameObject unlockNotification;
    public Image fragmentImage;
    public Image unlockImage;
    public float duration = 2f;

    void Awake()
    {
        // 如果已有实例，销毁重复的
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
            SetPreserveAspect(fragmentImage);
            float brightness = 0.5f;
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
        gameObject.SetActive(false);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;  // 引入 TextMesh Pro 命名空间
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject countdownPanel;  // 用来显示倒计时的 Panel（假设有一个显示文本的面板）

    [Header("Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button homeButton;
    public Button pauseButton;  // 用于手机上的暂停按钮

    [Header("Countdown Text")]
    public TMP_Text countdownText;  // 使用 TMP_Text 替代 Text 组件

    [Header("Buttons")]
    public Button backpackButton; // 背包按钮

    [Header("Backpack Icon")]
    public Image backpackIcon;
    public Sprite normalIcon;
    public Sprite redDotIcon;  

    private bool isCountingDown = false;  // 防止倒计时重复触发

    // 添加一个 BackpackManager 引用
    public BackpackManager backpackManager;

    void Start()
    {
        // 确保开始时是关闭暂停面板和倒计时面板
        pausePanel.SetActive(false);
        countdownPanel.SetActive(false);  // 倒计时面板一开始关闭

        // 绑定按钮事件
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        homeButton.onClick.AddListener(ReturnToMainMenu);
        pauseButton.onClick.AddListener(PauseGame);  // 绑定暂停按钮点击事件

        backpackButton.onClick.AddListener(OnBackpackButtonClicked);

        // 确保初始化时已经给 backpackManager 赋值
        if (backpackManager == null)
        {
            backpackManager = BackpackManager.Instance;  // 使用单例模式初始化
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;  // 停止游戏
        pausePanel.SetActive(true);  // 显示暂停菜单
        countdownPanel.SetActive(false);  // 不显示倒计时面板直到恢复游戏
    }

    public void ResumeGame()
    {
        if (isCountingDown) return;  // 如果已经在倒计时，则不重复调用

        isCountingDown = true;  // 开始倒计时
        pausePanel.SetActive(false);  // 隐藏暂停菜单
        countdownPanel.SetActive(true);  // 显示倒计时面板
        StartCoroutine(ResumeGameCountdown());  // 启动倒计时恢复游戏
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;  // 确保游戏恢复正常速度
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");  // 替换为你主菜单的场景名
    }

    private IEnumerator ResumeGameCountdown()
    {
        countdownText.gameObject.SetActive(true);  // 显示倒计时文本

        int count = 3;  // 倒计时开始值
        while (count > 0)
        {
            countdownText.text = count.ToString();  // 更新倒计时文本

            countdownText.transform.localScale = Vector3.one * 1.5f;  // 放大动画

            // 放大动画：简单的平滑放大效果
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * 5f;  // 使用 unscaledDeltaTime 保证动画不受时间缩放影响
                float scale = Mathf.Lerp(1.5f, 1f, t);
                countdownText.transform.localScale = Vector3.one * scale;  // 逐步缩小到正常大小
                yield return null;
            }

            yield return new WaitForSecondsRealtime(1f);  // 等待 1 秒，使用 real-time 等待
            count--;  // 减少倒计时
        }

        countdownPanel.SetActive(false);  // 隐藏倒计时面板
        Time.timeScale = 1f;  // 游戏继续
        isCountingDown = false;  // 重置倒计时标志
    }

    /// <summary>
    /// 设置背包图标状态（是否有新合成宝物提示）
    /// </summary>
    /// <param name="hasNew">是否有可合成的宝物</param>
    public void SetBackpackHasNewCraft(bool hasNew)
    {
        backpackIcon.sprite = hasNew ? redDotIcon : normalIcon;
    }

    /// <summary>
    /// 当点击背包按钮时打开背包界面，并通知 BackpackManager
    /// </summary>
    public void OnBackpackButtonClicked()
    {
        if (backpackManager != null)
        {
            backpackManager.OpenBackpack();  // 打开背包界面
        }
    }
}
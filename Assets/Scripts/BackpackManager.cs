using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class BackpackManager : MonoBehaviour
{
    public GameObject backpackUI;
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;

    public static BackpackManager Instance;

    public Action OnBackpackOpened; // 用于通知 UI Controller 刷新
    public Action OnFragmentsUpdated; // 用于通知 UI Controller 刷新单元格

    public void OpenBackpack()
    {
        backpackUI.SetActive(true);
        Time.timeScale = 0f;
        OnBackpackOpened?.Invoke(); // 通知 UI Controller 刷新
    }

    public void CloseBackpack()
    {
        backpackUI.SetActive(false);
        StartCoroutine(ResumeGameCountdown());
    }

    private IEnumerator ResumeGameCountdown()
    {
        countdownPanel.SetActive(true);
        countdownText.gameObject.SetActive(true);

        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            countdownText.transform.localScale = Vector3.one * 1.5f;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * 5f;
                float scale = Mathf.Lerp(1.5f, 1f, t);
                countdownText.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        countdownPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void NotifyFragmentGained()
    {
        OnFragmentsUpdated?.Invoke();
    }
}
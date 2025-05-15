using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    void Start()
    {
        // Only play main-menu music if the player hasn’t muted it
        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        if (musicOn)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.mainMenuMusic);
        }
    }

    public void PlayGame()
    {
        // Play button click SFX only if SFX is unmuted
        bool sfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;
        if (sfxOn)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);

        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelManager : MonoBehaviour
{
    [Header("Music Buttons & Sprites")]
    public Button musicOnButton;
    public Button musicOffButton;
    public Sprite musicActiveSprite;    
    public Sprite musicInactiveSprite;  

    [Header("SFX Buttons & Sprites")]
    public Button sfxOnButton;
    public Button sfxOffButton;
    public Sprite sfxActiveSprite;
    public Sprite sfxInactiveSprite;

    private bool isMusicOn;
    private bool isSfxOn;

    
    void Start()
    {
        // Load saved preferences (default: ON)
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;

        // Apply audio settings
        AudioManager.Instance.SetMusicVolume(isMusicOn ? 1 : 0);
        AudioManager.Instance.SetSFXVolume(isSfxOn ? 1 : 0);

        // Update UI visuals
        UpdateMusicVisuals();
        UpdateSfxVisuals();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        AudioManager.Instance.SetMusicVolume(isMusicOn ? 1 : 0);
        UpdateMusicVisuals();
    }

    public void ToggleSfx()
    {
        isSfxOn = !isSfxOn;
        PlayerPrefs.SetInt("SfxOn", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();

        AudioManager.Instance.SetSFXVolume(isSfxOn ? 1 : 0);
        UpdateSfxVisuals();
    }

    private void UpdateMusicVisuals()
    {
        musicOnButton.image.sprite = isMusicOn ? musicActiveSprite : musicInactiveSprite;
        musicOffButton.image.sprite = isMusicOn ? musicInactiveSprite : musicActiveSprite;
    }

    private void UpdateSfxVisuals()
    {
        sfxOnButton.image.sprite = isSfxOn ? sfxActiveSprite : sfxInactiveSprite;
        sfxOffButton.image.sprite = isSfxOn ? sfxInactiveSprite : sfxActiveSprite;
    }
}
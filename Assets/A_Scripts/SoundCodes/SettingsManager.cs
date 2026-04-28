using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;

    private void Start()
    {
        // 1. Kaydedilmiş değerleri yükle (Eğer daha önce ayar yapılmadıysa 1f yani %100 yap)
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 1f);
        float savedVoice = PlayerPrefs.GetFloat("VoiceVol", 1f);

        // 2. Slider'ların görsel konumunu güncelle
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;
        voiceSlider.value = savedVoice;

        // 3. Sesleri ilk açılışta hemen uygula
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
        SetVoiceVolume(savedVoice);
    }

    // Bu metodları Slider'ların OnValueChanged kısmına bağlayacağız
    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.bgmSource.volume = value;
            
        PlayerPrefs.SetFloat("MusicVol", value);
    }

    public void SetSFXVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.sfxSource.volume = value;
            
        PlayerPrefs.SetFloat("SFXVol", value);
    }

    public void SetVoiceVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.voSource.volume = value;
            
        PlayerPrefs.SetFloat("VoiceVol", value);
    }
}
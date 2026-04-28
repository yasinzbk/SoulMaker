using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Ses Kaynakları")]
    public AudioSource bgmSource;       // Arka plan müziği
    public AudioSource sfxSource;       // Buton klik vb.
    public AudioSource voSource;        // Karakter seslendirmeleri (Voice-Over)

    [Header("Genel Sesler")]
    public AudioClip defaultBGM;
    public AudioClip buttonClickClip;
    public AudioClip buttonHoverClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        if (defaultBGM != null) PlayBGM(defaultBGM);
    }

    // UI butonları için hazır metodlar
    public void PlayClickSound() { PlaySFX(buttonClickClip); }
    public void PlayHoverSound() { PlaySFX(buttonHoverClip); }

    // --- 3. DİYALOG SESLERİ ---
    public void PlayDialogueSound(AudioClip voiceClip)
    {
        if (voiceClip == null) return;

        voSource.clip = voiceClip;
        voSource.loop = true; // Yazı akarken sürekli tekrar etsin
        voSource.Play();
    }

    public void StopDialogueSound()
    {
        voSource.Stop();
    }



    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void PlayVO(AudioClip clip)
    {
        if (clip == null) return;

        voSource.Stop(); // Önceki sesi durdur
        voSource.clip = clip;
        voSource.loop = false; // Cümle bitince dursun
        voSource.Play();
    }

    public void StopVO() { voSource.Stop(); }
}
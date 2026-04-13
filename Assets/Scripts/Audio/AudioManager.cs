using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip charmCollectSfx;
    public AudioClip jumpSfx;
    public AudioClip dashSfx;
    public AudioClip hazardSfx;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SaveData data = SaveSystem.Load();
        AudioListener.volume = data.masterVolume;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayGameMusic() => PlayMusic(gameMusic);

    public void PlayCharmCollect() => PlaySfx(charmCollectSfx);
    public void PlayJump() => PlaySfx(jumpSfx);
    public void PlayDash() => PlaySfx(dashSfx);
    public void PlayHazard() => PlaySfx(hazardSfx);

    public void SetVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }
}

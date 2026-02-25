using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider fxSlider;
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip fx_hit;
    [Header("Volumes")]
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private bool musicMuted = false;
    private bool sfxMuted = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Apply initial volume
        UpdateMusicVolume();
        UpdateSFXVolume();
    }
    
    
    #region Music Controls

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void EnableMusic()
    {
        if (musicMuted)
        {
            musicMuted = false;
            PlayMusic(menuMusic,true);
        }
        else
        {
            musicMuted = true;
            StopMusic();
        }
    }

    public void EnableSFX()
    {
        if (sfxMuted)
        {
            sfxMuted = false;
            PlaySFX(fx_hit);
        }
        else
        {
            sfxMuted = true;
        }
    } 

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume()
    {
        musicVolume = Mathf.Clamp01(musicSlider.value);
        UpdateMusicVolume();
    }

    public void MuteMusic(bool mute)
    {
        musicMuted = mute;
        UpdateMusicVolume();
    }

    private void UpdateMusicVolume()
    {
        musicSource.volume = musicMuted ? 0f : musicVolume;
    }

    #endregion

    #region SFX Controls

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip, sfxMuted ? 0f : sfxVolume);
    }

    public void SetSFXVolume()
    {
        sfxVolume = Mathf.Clamp01(fxSlider.value);
        UpdateSFXVolume();
        if(!sfxSource.isPlaying)
            PlaySFX(fx_hit);
    }

    public void MuteSFX(bool mute)
    {
        sfxMuted = mute;
        UpdateSFXVolume();
    }

    private void UpdateSFXVolume()
    {
        sfxSource.volume = sfxMuted ? 0f : sfxVolume;
    }

    #endregion

    #region Global Controls

    public void MuteAll(bool mute)
    {
        MuteMusic(mute);
        MuteSFX(mute);
    }

    #endregion
}

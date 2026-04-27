using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider fxSlider;
    [Header("Audio Sources")] 
    public AudioSource musicSource;//quitar(?
    public AudioSource sfxSource;
    public AudioClip menuMusic;//quitar(?
    public AudioClip fx_hit;
    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private bool musicMuted = false;
    private bool sfxMuted = false;

    void Awake()
    {
        // Apply initial volume
        //UpdateMusicVolume();
        UpdateSFXVolume();
    }

    //-- Music audio
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
            PlayMusic(menuMusic, true);
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
        SaveVolume();
    }


    private void UpdateMusicVolume()
    {
        musicSource.volume = musicMuted ? 0f : musicVolume;
    }

//-SFX Audio
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip, sfxMuted ? 0f : sfxVolume);
    }

    public void SetSFXVolume()
    {
        sfxVolume = Mathf.Clamp01(fxSlider.value);
        UpdateSFXVolume();
        SaveVolume();
        if (!sfxSource.isPlaying)
            PlaySFX(fx_hit);
    }

    // public void MuteSFX(bool mute)
    // {
    //     sfxMuted = mute;
    //     UpdateSFXVolume();
    // }

    private void UpdateSFXVolume()
    {
        sfxSource.volume = sfxMuted ? 0f : sfxVolume;
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.Save();
    }
}

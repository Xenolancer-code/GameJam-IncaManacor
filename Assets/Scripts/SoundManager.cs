using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider fxSlider;
    [Header("Audio Sources")] 
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource menuOptionsSource;
    [Header("Audio Clips")]
    public AudioClip[] menuMusicList;
    public AudioClip fx_hit;
    public AudioClip fx_MenuLibro;
    public AudioClip fx_MenuGramofono;
    public AudioClip fx_MenuAbout;
    public AudioClip fx_MenuExit;
    
    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private bool musicMuted = false;
    private bool sfxMuted = false;
    private Coroutine musicCoroutine;
    private float lastMusicIndex;

    void Awake()
    {
        // Apply initial volume
        //UpdateMusicVolume();
        UpdateSFXVolume();
    }
    void Start()
    {
        if (menuMusicList != null && menuMusicList.Length > 0)
            musicCoroutine = StartCoroutine(PlayMusicPlaylist());
    }
    private IEnumerator PlayMusicPlaylist()
    {
        while (!musicMuted)
        {
            int index = GetRandomMusicIndex();
            lastMusicIndex = index;

            musicSource.clip = menuMusicList[index];
            musicSource.loop = false;
            musicSource.Play();

            // Espera a que acabe el clip
            yield return new WaitForSeconds(musicSource.clip.length);
        }
    }
    
    private int GetRandomMusicIndex()
    {
        if (menuMusicList.Length == 1) return 0;

        int index;
        do { index = Random.Range(0, menuMusicList.Length); }
        while (index == lastMusicIndex);

        return index;
    }
    
    //-- Music audio
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    public void PlayMenuOptionsMusic (int index, bool loop = true)
    {
        switch (index)
        {
            case 0:
                menuOptionsSource.clip = fx_MenuLibro;
                break;
            case 1:
                menuOptionsSource.clip = fx_MenuGramofono;
                break;
            case 2:
                menuOptionsSource.clip = fx_MenuAbout;
                break;
            case 3:
                menuOptionsSource.clip = fx_MenuExit;
                break;
        }
        menuOptionsSource.loop = loop;
        menuOptionsSource.Play();
    }


    public void StopMenuOptionsMusic()
    {
        menuOptionsSource.Stop();
    }

    public void EnableMusic()
    {
        if (musicMuted)
        {
            musicMuted = false;
            musicCoroutine = StartCoroutine(PlayMusicPlaylist());
        }
        else
        {
            musicMuted = true;
            if (musicCoroutine != null) StopCoroutine(musicCoroutine);
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

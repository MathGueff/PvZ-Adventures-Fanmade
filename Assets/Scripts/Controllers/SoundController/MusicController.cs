using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    public AudioSource musicSource;
    public AudioClip[] musicClips; // Array de músicas

    private int currentMusicIndex = -1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Faz o MusicController persistir entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayRandomMusic()
    {
        if (musicClips.Length > 0)
        {
            int newMusicIndex = Random.Range(0, musicClips.Length);
            while (newMusicIndex == currentMusicIndex)
            {
                newMusicIndex = Random.Range(0, musicClips.Length);
            }

            currentMusicIndex = newMusicIndex;
            PlayMusic(musicClips[currentMusicIndex]);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.clip = musicClips[currentMusicIndex];
            musicSource.Play();
        }
    }

    public void StopCurrentMusic()
    {
        musicSource.Stop();
    }

    public void SetIsLoop(bool boolean)
    {
        musicSource.loop = boolean;
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }
}

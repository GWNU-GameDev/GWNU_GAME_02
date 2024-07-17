using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [FMODUnity.EventRef]
    public string backgroundMusicEvent;
    [FMODUnity.EventRef]
    public string sfxEvent;

    private FMOD.Studio.EventInstance backgroundMusicInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void PlayBackgroundMusic(string eventPath)
    {
        if (backgroundMusicInstance.isValid())
        {
            backgroundMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            backgroundMusicInstance.release();
        }

        backgroundMusicInstance = RuntimeManager.CreateInstance(eventPath);
        backgroundMusicInstance.start();
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusicInstance.isValid())
        {
            backgroundMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            backgroundMusicInstance.release();
        }
    }

    public void PlaySFX(string eventPath)
    {
        RuntimeManager.PlayOneShot(eventPath);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private void Awake()
    {
        // 싱글턴 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 사운드 이벤트 재생
    public void PlaySound(string path)
    {
        RuntimeManager.PlayOneShot(path);
    }

    // 사운드 이벤트 재생 위치 지정
    public void PlaySoundAtPosition(string path, Vector3 position)
    {
        RuntimeManager.PlayOneShot(path, position);
    }

    // 사운드 이벤트 볼륨 조절
    public void SetVolume(FMOD.Studio.EventInstance soundEvent, float volume)
    {
        soundEvent.setVolume(volume);
    }

    // 사운드 이벤트 중지
    public void StopSound(FMOD.Studio.EventInstance soundEvent, bool immediate = false)
    {
        if (immediate)
        {
            soundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        else
        {
            soundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private Bus masterBus;
    private float previousVolume = 1f;
    private bool isMuted = false;

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

        masterBus = RuntimeManager.GetBus("bus:/");
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
    public void SetVolume(EventInstance soundEvent, float volume)
    {
        soundEvent.setVolume(volume);
    }

    // 마스터 볼륨 조절
    public void SetMasterVolume(float volume)
    {
        masterBus.setVolume(volume);
    }

    // 사운드 이벤트 중지
    public void StopSound(EventInstance soundEvent, bool immediate = false)
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

    // 음소거 토글 (볼륨 0으로 설정)
    public void ToggleMute()
    {
        if (isMuted)
        {
            masterBus.setVolume(previousVolume);
        }
        else
        {
            masterBus.getVolume(out previousVolume);
            masterBus.setVolume(0f);
        }

        isMuted = !isMuted;
    }
}

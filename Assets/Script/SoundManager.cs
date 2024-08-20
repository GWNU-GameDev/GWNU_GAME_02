using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private Bus masterBus;
    private Bus sfxBus;
    private float previousMasterVolume = 1f;
    private float previousSFXVolume = 1f;
    private bool isMasterMuted = false;
    private bool isSFXMuted = false;

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
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
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
        previousMasterVolume = volume;
    }

    // SFX 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        sfxBus.setVolume(volume);
        previousSFXVolume = volume;
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

    // 마스터 음소거 토글
    public void ToggleMasterMute()
    {
        if (isMasterMuted)
        {
            masterBus.setVolume(previousMasterVolume);
        }
        else
        {
            masterBus.getVolume(out previousMasterVolume);
            masterBus.setVolume(0f);
        }
        isMasterMuted = !isMasterMuted;
    }

    // SFX 음소거 토글
    public void ToggleSFXMute()
    {
        if (isSFXMuted)
        {
            sfxBus.setVolume(previousSFXVolume);
        }
        else
        {
            sfxBus.getVolume(out previousSFXVolume);
            sfxBus.setVolume(0f);
        }
        isSFXMuted = !isSFXMuted;
    }

    // 현재 마스터 볼륨 가져오기
    public float GetMasterVolume()
    {
        float volume;
        masterBus.getVolume(out volume);
        return volume;
    }

    // 현재 SFX 볼륨 가져오기
    public float GetSFXVolume()
    {
        float volume;
        sfxBus.getVolume(out volume);
        return volume;
    }
}
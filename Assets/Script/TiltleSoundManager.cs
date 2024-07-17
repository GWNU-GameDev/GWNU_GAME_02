using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string titleMusicEvent;

    private void Start()
    {
        SoundManager.Instance.PlayBackgroundMusic(titleMusicEvent);
    }

    private void OnDestroy()
    {
        SoundManager.Instance.StopBackgroundMusic();
    }
}
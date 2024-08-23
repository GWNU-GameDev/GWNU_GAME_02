using UnityEngine;
using FMODUnity;
using System.Collections;

public class TitleMusicManager : MonoBehaviour
{
    [SerializeField]
    private string tutorialSoundPath = "event:/TutorialSound";
    [SerializeField]
    private string backgroundMusicPath = "event:/BackgroundMusic";

    private FMOD.Studio.EventInstance tutorialSound;
    private FMOD.Studio.EventInstance backgroundMusic;

    void Start()
    {
        // 튜토리얼 사운드 이벤트 인스턴스 생성
        tutorialSound = RuntimeManager.CreateInstance(tutorialSoundPath);

        // 배경음악 이벤트 인스턴스 생성
        backgroundMusic = RuntimeManager.CreateInstance(backgroundMusicPath);

        // 코루틴 시작
        StartCoroutine(PlayMusicSequence());
    }

    IEnumerator PlayMusicSequence()
    {
        // 튜토리얼 사운드 재생
        tutorialSound.start();

        // 50초 대기
        yield return new WaitForSeconds(50f);

        // 튜토리얼 사운드 정지
        tutorialSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        tutorialSound.release();

        // 배경음악 재생
        backgroundMusic.start();
    }

    void OnDestroy()
    {
        // 씬이 언로드 될 때 모든 음악 중지 및 리소스 해제
        tutorialSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        tutorialSound.release();

        backgroundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        backgroundMusic.release();
    }
}
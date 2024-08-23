using UnityEngine;
using FMODUnity;
public class TitleMusicManager : MonoBehaviour
{
    [SerializeField]
    private string musicEventPath = "event:/BackgroundMusic";
    private FMOD.Studio.EventInstance musicEvent;
    void Start()
    {
        // 배경음악 이벤트 인스턴스 생성
        musicEvent = RuntimeManager.CreateInstance(musicEventPath);

        // 배경음악 재생
        musicEvent.start();
    }
    void OnDestroy()
    {
        // 씬이 언로드 될 때 음악 중지 및 리소스 해제
        musicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicEvent.release();
    }
}
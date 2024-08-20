using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Slider masterVolumeSlider;    // 마스터 볼륨 조절 슬라이더
    public Slider sfxVolumeSlider;       // SFX 볼륨 조절 슬라이더
    public Slider mouseSpeedSlider;      // 마우스 감도 조절 슬라이더
    public Toggle masterMuteToggle;      // 마스터 음소거 토글
    public Toggle sfxMuteToggle;         // SFX 음소거 토글
    public Button resumeButton;          // 게임 재개 버튼
    public Button quitButton;            // 게임 종료 버튼
    private bool isPaused = false;       // 게임 일시정지 상태
    private CameraController cameraController;

    void Start()
    {
        // 초기에는 일시정지 메뉴를 비활성화
        pauseMenuUI.SetActive(false);

        // UI 요소들에 리스너 추가
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        mouseSpeedSlider.onValueChanged.AddListener(SetMouseSpeed);
        masterMuteToggle.onValueChanged.AddListener(ToggleMasterMute);
        sfxMuteToggle.onValueChanged.AddListener(ToggleSFXMute);
        resumeButton.onClick.AddListener(Resume);
        quitButton.onClick.AddListener(QuitGame);

        // 마우스 감도 슬라이더 설정
        mouseSpeedSlider.minValue = 200f;
        mouseSpeedSlider.maxValue = 600f;
        mouseSpeedSlider.value = 400f;

        // 현재 볼륨 값으로 슬라이더 초기화
        masterVolumeSlider.value = SoundManager.Instance.GetMasterVolume();
        sfxVolumeSlider.value = SoundManager.Instance.GetSFXVolume();

        // 게임 시작 시 마우스 포인터 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // CameraController 컴포넌트 찾기
        cameraController = FindObjectOfType<CameraController>();
        if (cameraController == null)
        {
            Debug.LogWarning("CameraController를 찾을 수 없습니다.");
        }
        else
        {
            cameraController.SetMouseSpeed(mouseSpeedSlider.value);
        }
    }

    void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC 키가 눌렸습니다");
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // 게임 재개
    void Resume()
    {
        Debug.Log("게임 재개");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 게임 일시정지
    void Pause()
    {
        Debug.Log("게임 일시정지");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 마스터 볼륨 설정
    void SetMasterVolume(float volume)
    {
        SoundManager.Instance.SetMasterVolume(volume);
    }

    // SFX 볼륨 설정
    void SetSFXVolume(float volume)
    {
        SoundManager.Instance.SetSFXVolume(volume);
    }

    // 마스터 음소거 토글
    void ToggleMasterMute(bool isMuted)
    {
        SoundManager.Instance.ToggleMasterMute();
    }

    // SFX 음소거 토글
    void ToggleSFXMute(bool isMuted)
    {
        SoundManager.Instance.ToggleSFXMute();
    }

    // 마우스 감도 설정
    void SetMouseSpeed(float speed)
    {
        if (cameraController != null)
        {
            cameraController.SetMouseSpeed(speed);
        }
    }

    // 게임 종료
    void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
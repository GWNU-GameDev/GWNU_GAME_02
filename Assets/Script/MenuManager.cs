using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Slider volumeSlider;
    public Slider mouseSpeedSlider; // 추가된 슬라이더
    public Toggle muteToggle;
    public Button resumeButton;
    public Button quitButton;
    private bool isPaused = false;

    private CameraController cameraController;

    void Start()
    {
        // 초기에는 설정창을 비활성화
        pauseMenuUI.SetActive(false);

        // UI 요소들에 리스너 추가
        volumeSlider.onValueChanged.AddListener(SetVolume);
        mouseSpeedSlider.onValueChanged.AddListener(SetMouseSpeed); // 추가된 리스너
        muteToggle.onValueChanged.AddListener(ToggleMute);
        resumeButton.onClick.AddListener(Resume);
        quitButton.onClick.AddListener(QuitGame);

        // 슬라이더 값 설정
        mouseSpeedSlider.minValue = 200f; // 최소값 200
        mouseSpeedSlider.maxValue = 600f; // 최대값 600
        mouseSpeedSlider.value = 400f;    // 기본값 400

        // 게임 시작 시 마우스 포인터 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraController = FindObjectOfType<CameraController>();
        if (cameraController == null)
        {
            Debug.LogWarning("CameraController not found in the scene.");
        }
        else
        {
            cameraController.SetMouseSpeed(mouseSpeedSlider.value);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC 키가 눌렸습니다"); // ESC 키 입력 감지 확인 로그
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

    void Resume()
    {
        Debug.Log("게임 재개"); // 재개 확인 로그
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // 마우스 포인터 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        Debug.Log("게임 일시정지"); // 일시정지 확인 로그
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // 마우스 포인터 보이기
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void SetVolume(float volume)
    {
        SoundManager.Instance.SetMasterVolume(volume);
    }

    void ToggleMute(bool isMuted)
    {
        SoundManager.Instance.ToggleMute();
    }

    void SetMouseSpeed(float speed)
    {
        if (cameraController != null)
        {
            cameraController.SetMouseSpeed(speed);
        }
    }

    void QuitGame()
    {
        Debug.Log("게임 종료"); // 종료 확인 로그
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Slider volumeSlider;
    public Toggle muteToggle;
    public Button resumeButton;
    public Button quitButton;
    private bool isPaused = false;

    void Start()
    {
        // 초기에는 설정창을 비활성화
        pauseMenuUI.SetActive(false);

        // UI 요소들에 리스너 추가
        volumeSlider.onValueChanged.AddListener(SetVolume);
        muteToggle.onValueChanged.AddListener(ToggleMute);
        resumeButton.onClick.AddListener(Resume);
        quitButton.onClick.AddListener(QuitGame);

        // 초기 볼륨 설정
        volumeSlider.value = 1f;

        // 게임 시작 시 마우스 포인터 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    void QuitGame()
    {
        Debug.Log("게임 종료"); // 종료 확인 로그
        Application.Quit();
    }
}

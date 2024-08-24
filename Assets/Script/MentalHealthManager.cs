using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MentalHealthManager : MonoBehaviour
{
    public static MentalHealthManager Instance { get; private set; } // 싱글톤 인스턴스
    public int maxMentalHealth = 100;    // 최대 정신 건강 수치
    public int decreaseRate = 1;         // 정신 건강 감소율
    private int currentMentalHealth = 100;// 현재 정신 건강 수치
    public GameObject level2Object;      // 레벨 2 오브젝트
    public GameObject level3Object;      // 레벨 3 오브젝트
    private EyesBlinking eyesBlinking;   // 눈 깜빡임 컴포넌트
    private float immunityDuration = 50f; // 면역 지속 시간
    private float immunityTimer;         // 면역 타이머

    // 엔딩 씬 이름 배열
    private string[] endingScenes = { "Mental0EndingScene_01", "Mental0EndingScene_02" };

    // 사운드 이벤트 경로
    private const string MENTAL1_SOUND_EVENT = "event:/Mental1";
    private const string MENTAL2_SOUND_EVENT = "event:/Mental2";
    private const string MENTAL3_SOUND_EVENT = "event:/Mental3";
    private const string CHANGE_MENTAL_SOUND_EVENT = "event:/ChangeMental";

    private int lastSoundPlayedAt;       // 마지막으로 사운드가 재생된 정신 건강 수치
    private int currentMentalLevel = 1;  // 현재 정신 건강 레벨

    private void Awake()
    {
        // 싱글톤 패턴 구현
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

    void Start()
    {
        currentMentalHealth = maxMentalHealth;
        eyesBlinking = FindObjectOfType<EyesBlinking>();
        if (eyesBlinking == null)
        {
            Debug.LogError("EyesBlinking 컴포넌트를 씬에서 찾을 수 없습니다!");
        }
        UpdateMentalHealthUI();
        UpdateLevelStatus();
        immunityTimer = immunityDuration;
        StartCoroutine(ImmunityCountdown());
        lastSoundPlayedAt = currentMentalHealth;
    }

    // UI 업데이트 (현재는 로그만 출력)
    void UpdateMentalHealthUI()
    {
        Debug.Log("현재 정신 건강: " + currentMentalHealth);
    }

    // 정신 건강 감소 메서드
    public void DecreaseMentalHealth()
    {
        if (immunityTimer > 0)
        {
            Debug.Log("면역 상태 활성화. 정신 건강이 감소하지 않습니다.");
            return;
        }
        int previousMentalHealth = currentMentalHealth;
        currentMentalHealth -= decreaseRate;
        if (currentMentalHealth <= 0)
        {
            currentMentalHealth = 0;
            LoadRandomEndingScene();
        }
        UpdateMentalHealthUI();
        if (ShouldForceBlinkEyes(previousMentalHealth, currentMentalHealth))
        {
            StartCoroutine(ForceBlinkAndUpdateLevel());
        }
        else
        {
            UpdateLevelStatus();
        }

        PlayMentalHealthSound();
    }

    // 정신 건강 사운드 재생
    private void PlayMentalHealthSound()
    {
        if (currentMentalHealth <= lastSoundPlayedAt - 10)
        {
            lastSoundPlayedAt = currentMentalHealth;
            string soundToPlay = DetermineSoundToPlay();
            if (SoundManager.Instance != null && !string.IsNullOrEmpty(soundToPlay))
            {
                SoundManager.Instance.PlaySound(soundToPlay);
            }
        }
    }

    // 재생할 사운드 결정
    private string DetermineSoundToPlay()
    {
        if (currentMentalHealth >= 75 && currentMentalHealth <= 100)
        {
            return MENTAL1_SOUND_EVENT;
        }
        else if (currentMentalHealth >= 50 && currentMentalHealth <= 74)
        {
            return MENTAL2_SOUND_EVENT;
        }
        else if (currentMentalHealth >= 1 && currentMentalHealth <= 49)
        {
            return MENTAL3_SOUND_EVENT;
        }
        return "";
    }

    // 강제로 눈을 깜빡여야 하는지 확인
    private bool ShouldForceBlinkEyes(int previousHealth, int currentHealth)
    {
        return (previousHealth > 74 && currentHealth == 74) ||
               (previousHealth > 49 && currentHealth <= 49 && previousHealth > 49);
    }

    // 강제 눈 깜빡임 및 레벨 업데이트
    private IEnumerator ForceBlinkAndUpdateLevel()
    {
        yield return StartCoroutine(eyesBlinking.ForceBlinkCoroutine());
        UpdateLevelStatus();
    }

    // 정신 건강이 고갈되었는지 확인
    public bool IsMentalHealthDepleted()
    {
        return currentMentalHealth <= 0;
    }

    // 정신 건강 비율 반환
    public float GetMentalHealthPercentage()
    {
        return (float)currentMentalHealth / maxMentalHealth;
    }

    // 레벨 상태 업데이트
    private void UpdateLevelStatus()
    {
        int newMentalLevel;

        if (currentMentalHealth >= 75 && currentMentalHealth <= 100)
        {
            newMentalLevel = 1;
            level2Object.SetActive(false);
            level3Object.SetActive(false);
        }
        else if (currentMentalHealth >= 50 && currentMentalHealth <= 74)
        {
            newMentalLevel = 2;
            level2Object.SetActive(true);
            level3Object.SetActive(false);
        }
        else
        {
            newMentalLevel = 3;
            level2Object.SetActive(false);
            level3Object.SetActive(true);
        }

        // 레벨이 변경되었는지 확인
        if (newMentalLevel != currentMentalLevel)
        {
            currentMentalLevel = newMentalLevel;
            PlayChangeMentalSound();
        }
    }

    // ChangeMental 사운드 재생
    private void PlayChangeMentalSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(CHANGE_MENTAL_SOUND_EVENT);
        }
        else
        {
            Debug.LogWarning("SoundManager 인스턴스를 찾을 수 없습니다. ChangeMental 사운드를 재생할 수 없습니다.");
        }
    }

    // 면역 시간 카운트다운
    private IEnumerator ImmunityCountdown()
    {
        while (immunityTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            immunityTimer--;
            Debug.Log($"남은 면역 시간: {immunityTimer} 초");
        }
        Debug.Log("면역 기간이 종료되었습니다. 이제 정신 건강이 감소할 수 있습니다.");
    }

    // 랜덤 엔딩 씬 로드
    private void LoadRandomEndingScene()
    {
        string randomEndingScene = endingScenes[Random.Range(0, endingScenes.Length)];
        SceneManager.LoadScene(randomEndingScene);
        Debug.Log($"엔딩 씬 로드 중: {randomEndingScene}");
    }
}
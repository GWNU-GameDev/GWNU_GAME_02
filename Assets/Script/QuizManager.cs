using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance { get; private set; } // 싱글톤 인스턴스

    public GameObject quizPanel; // 퀴즈 패널 UI
    public TextMeshProUGUI questionText; // 문제 텍스트
    public TextMeshProUGUI[] answerTexts; // 답변 텍스트 배열

    private List<WeightedImageQuizItem> currentQuizItems; // 현재 퀴즈 아이템 리스트
    private int currentQuizIndex = 0; // 현재 퀴즈 인덱스
    private int correctAnswerIndex = -1; // 정답 인덱스
    private bool quizActive = false; // 퀴즈 활성화 상태
    private float remainingTime = 20f; // 남은 시간
    private int hp = 5; // 플레이어 체력
    private Coroutine timerCoroutine; // 타이머 코루틴

    private const string DAMAGE_SOUND_EVENT = "event:/Damage"; // 데미지 사운드 이벤트 경로
    private const string STEP_SOUND_EVENT = "event:/StepSound"; // 발소리 사운드 이벤트 경로

    private int incorrectAnswers = 0; // 틀린 답변 수

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

    private void Start()
    {
        // 퀴즈 패널 초기 비활성화
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("퀴즈 패널이 Inspector에서 할당되지 않았습니다.");
        }
    }

    // 퀴즈 시작 메서드
    public void StartQuiz(List<WeightedImageQuizItem> shownImages)
    {
        currentQuizItems = GetRandomQuizItems(shownImages, 3);
        currentQuizIndex = 0;
        quizActive = true;

        quizPanel.SetActive(true);

        // 발소리 재생
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(STEP_SOUND_EVENT);
        }
        else
        {
            Debug.LogWarning("SoundManager 인스턴스를 찾을 수 없습니다. 제대로 초기화되었는지 확인하세요.");
        }

        // 퀴즈 시작 시 면역 상태 시작
        MentalHealthManager.Instance.StartImmunity();

        SetNextQuiz();
        remainingTime = 20f;
        timerCoroutine = StartCoroutine(StartTimer());
        Debug.Log("퀴즈를 시작합니다. 20초 안에 문제를 풀어주세요.");
    }

    // 랜덤 퀴즈 아이템 선택
    private List<WeightedImageQuizItem> GetRandomQuizItems(List<WeightedImageQuizItem> sourceList, int count)
    {
        List<WeightedImageQuizItem> randomItems = new List<WeightedImageQuizItem>();
        List<WeightedImageQuizItem> tempList = new List<WeightedImageQuizItem>(sourceList);

        for (int i = 0; i < count && tempList.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            randomItems.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        return randomItems;
    }

    // 다음 퀴즈 설정
    void SetNextQuiz()
    {
        if (currentQuizIndex < currentQuizItems.Count)
        {
            WeightedImageQuizItem currentQuiz = currentQuizItems[currentQuizIndex];
            questionText.text = $"Photo {(currentQuiz.sequenceNumber - 1) % 5 + 1} : {currentQuiz.question}";

            List<string> allAnswers = new List<string> { currentQuiz.correctAnswer };
            allAnswers.AddRange(currentQuiz.wrongAnswers);
            ShuffleList(allAnswers);

            for (int i = 0; i < answerTexts.Length; i++)
            {
                answerTexts[i].text = $"{i + 1}. {allAnswers[i]}";
                if (allAnswers[i] == currentQuiz.correctAnswer)
                {
                    correctAnswerIndex = i;
                }
            }

            currentQuizIndex++;
        }
        else
        {
            EndQuiz();
        }
    }

    // 답변 확인
    public void CheckAnswer(int selectedAnswerIndex)
    {
        if (selectedAnswerIndex == correctAnswerIndex)
        {
            Debug.Log("정답!");
        }
        else
        {
            Debug.Log("오답!");
            incorrectAnswers++; // 틀린 답변 수 증가
            DecreaseHP();
        }

        if (currentQuizIndex < currentQuizItems.Count)
        {
            SetNextQuiz();
        }
        else
        {
            EndQuiz();
        }
    }

    // 퀴즈 종료
    void EndQuiz()
    {
        quizActive = false;

        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // 퀴즈 종료 시 면역 상태 해제
        MentalHealthManager.Instance.EndImmunity();

        // 퀴즈 종료 시 틀린 답변이 있으면 사운드 재생
        if (incorrectAnswers > 0 && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(DAMAGE_SOUND_EVENT);
        }

        if (hp <= 0)
        {
            GameOver();
        }

        // 퀴즈 종료 후 변수 초기화
        incorrectAnswers = 0;
    }

    // 리스트 섞기
    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // 타이머 시작
    IEnumerator StartTimer()
    {
        while (remainingTime > 0 && quizActive)
        {
            remainingTime -= Time.deltaTime;
            Debug.Log($"남은 시간: {Mathf.CeilToInt(remainingTime)}초");
            yield return null;
        }

        if (quizActive)
        {
            Debug.Log("시간 초과!");
            EndQuiz();
        }
    }

    // HP 감소
    void DecreaseHP()
    {
        hp--;
        Debug.Log($"현재 HP: {hp}");
    }

    // 게임 오버
    void GameOver()
    {
        Debug.Log("게임 오버!");
        // 게임 오버 시에도 면역 상태 해제
        MentalHealthManager.Instance.EndImmunity();
        SceneManager.LoadScene("Strength0EndingScene"); // 게임 오버 시 Strength0EndingScene으로 이동
    }

    // 퀴즈 활성화 상태 확인
    public bool IsQuizActive()
    {
        return quizActive;
    }

    // 매 프레임 실행
    void Update()
    {
        if (quizActive)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    CheckAnswer(i);
                    break;
                }
            }
        }
    }
}
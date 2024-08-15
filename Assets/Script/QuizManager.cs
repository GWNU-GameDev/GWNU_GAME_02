using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance { get; private set; }

    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI[] answerTexts;

    private List<WeightedImageQuizItem> currentQuizItems;
    private int currentQuizIndex = 0;
    private int correctAnswerIndex = -1;
    private bool quizActive = false;
    private float remainingTime = 20f;
    private int hp = 5;
    private Coroutine timerCoroutine;

    private void Awake()
    {
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
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Quiz panel is not assigned in the Inspector.");
        }
    }

    public void StartQuiz(List<WeightedImageQuizItem> shownImages)
    {
        currentQuizItems = GetRandomQuizItems(shownImages, 3);
        currentQuizIndex = 0;
        quizActive = true;

        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
        }

        SetNextQuiz();
        remainingTime = 20f;
        timerCoroutine = StartCoroutine(StartTimer());
        Debug.Log("퀴즈를 시작합니다. 20초 안에 3개의 문제를 풀어주세요.");
    }

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

    void SetNextQuiz()
    {
        if (currentQuizIndex < currentQuizItems.Count)
        {
            WeightedImageQuizItem currentQuiz = currentQuizItems[currentQuizIndex];
            questionText.text = $"{currentQuiz.sequenceNumber} {currentQuiz.question}";

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

    public void CheckAnswer(int selectedAnswerIndex)
    {
        if (selectedAnswerIndex == correctAnswerIndex)
        {
            Debug.Log("정답!");
        }
        else
        {
            Debug.Log("오답!");
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

        if (hp <= 0)
        {
            GameOver();
        }
    }

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

    IEnumerator StartTimer()
    {
        while (remainingTime > 0 && quizActive)
        {
            remainingTime -= Time.unscaledDeltaTime;
            Debug.Log($"남은 시간: {Mathf.CeilToInt(remainingTime)}초");
            yield return null;
        }

        if (quizActive)
        {
            Debug.Log("시간 초과!");
            EndQuiz();
        }
    }

    void DecreaseHP()
    {
        hp--;
        Debug.Log($"현재 HP: {hp}");
    }

    void GameOver()
    {
        Debug.Log("게임 오버!");
        // 여기에 게임 오버 로직 추가
    }

    public bool IsQuizActive()
    {
        return quizActive;
    }

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
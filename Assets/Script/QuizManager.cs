using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Serializable]
    public class QuizItem
    {
        public string question;
        public string correctAnswer;
        public string[] wrongAnswers = new string[3];
    }

    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI[] answerTexts; // 4개의 TMP 텍스트 요소를 위한 배열
    public List<QuizItem> quizzes = new List<QuizItem>();

    private int correctAnswerIndex = -1;
    private bool quizActive = false;
    private List<int> unusedQuizIndices = new List<int>();

    void Start()
    {
        quizPanel.SetActive(false);
        InitializeQuizIndices();
    }

    void InitializeQuizIndices()
    {
        unusedQuizIndices.Clear();
        for (int i = 0; i < quizzes.Count; i++)
        {
            unusedQuizIndices.Add(i);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartQuiz();
        }

        if (quizActive)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    CheckAnswer(i);
                }
            }
        }
    }

    public void StartQuiz()
    {
        if (quizPanel == null || unusedQuizIndices.Count == 0)
        {
            Debug.LogError("Quiz panel is not assigned or all questions have been used.");
            return;
        }

        quizPanel.SetActive(true);
        quizActive = true;
        Time.timeScale = 0f;
        SetRandomQuiz();
    }

    void SetRandomQuiz()
    {
        int quizRandomIndex = UnityEngine.Random.Range(0, unusedQuizIndices.Count);
        int quizIndex = unusedQuizIndices[quizRandomIndex];
        QuizItem currentQuiz = quizzes[quizIndex];

        questionText.text = currentQuiz.question;

        List<string> allAnswers = new List<string> { currentQuiz.correctAnswer };
        allAnswers.AddRange(currentQuiz.wrongAnswers);

        // 답변 옵션들을 섞습니다
        for (int i = 0; i < allAnswers.Count; i++)
        {
            string temp = allAnswers[i];
            int shuffleIndex = UnityEngine.Random.Range(i, allAnswers.Count);
            allAnswers[i] = allAnswers[shuffleIndex];
            allAnswers[shuffleIndex] = temp;
        }

        for (int i = 0; i < answerTexts.Length; i++)
        {
            answerTexts[i].text = $"{i + 1}. {allAnswers[i]}";
            if (allAnswers[i] == currentQuiz.correctAnswer)
            {
                correctAnswerIndex = i;
            }
        }

        unusedQuizIndices.RemoveAt(quizRandomIndex);
    }

    void CheckAnswer(int selectedAnswerIndex)
    {
        if (selectedAnswerIndex == correctAnswerIndex)
        {
            Debug.Log("Correct!");
        }
        else
        {
            Debug.Log("Wrong!");
        }

        quizActive = false;
        quizPanel.SetActive(false);
        Time.timeScale = 1f;

        if (unusedQuizIndices.Count == 0)
        {
            Debug.Log("All questions have been used. Resetting the quiz.");
            InitializeQuizIndices();
        }
    }
}
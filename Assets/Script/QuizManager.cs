using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public GameObject quizPanel; // 퀴즈 패널
    public List<Image> quizImages; // 퀴즈 이미지들
    public List<Sprite> imageOptions; // 이미지 옵션들
    public List<int> imageIndices; // 각 이미지 위치에 대한 인덱스
    public int correctAnswerIndex = -1; // 정답 인덱스 (-1이면 랜덤으로 설정)
    private bool quizActive = false;

    void Start()
    {
        quizPanel.SetActive(false); // 시작할 때 퀴즈 패널 비활성화
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartQuiz();
        }

        if (quizActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CheckAnswer(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CheckAnswer(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CheckAnswer(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CheckAnswer(3);
            }
        }
    }

    // 퀴즈를 시작하는 메서드
    public void StartQuiz()
    {
        if (quizPanel == null)
        {
            Debug.LogError("Quiz panel is not assigned.");
            return;
        }

        if (imageIndices.Count != quizImages.Count)
        {
            Debug.LogError("Image indices count must match quiz images count.");
            return;
        }

        quizPanel.SetActive(true);
        quizActive = true;
        SetQuizImages();
    }

    // 퀴즈 이미지를 설정하는 메서드
    void SetQuizImages()
    {
        for (int i = 0; i < quizImages.Count; i++)
        {
            int imageIndex = imageIndices[i];
            if (imageIndex >= 0 && imageIndex < imageOptions.Count)
            {
                quizImages[i].sprite = imageOptions[imageIndex];
            }
            else
            {
                Debug.LogError("Invalid image index: " + imageIndex);
            }
        }

        if (correctAnswerIndex < 0 || correctAnswerIndex >= quizImages.Count)
        {
            correctAnswerIndex = Random.Range(0, quizImages.Count); // 정답 인덱스 랜덤 설정
        }
    }

    // 정답을 확인하는 메서드
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
    }
}

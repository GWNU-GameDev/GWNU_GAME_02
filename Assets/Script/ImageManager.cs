using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 이미지 퀴즈 아이템을 위한 클래스
[System.Serializable]
public class WeightedImageQuizItem
{
    public Sprite image;            // 퀴즈에 사용될 이미지
    public string question;         // 퀴즈 질문
    public string correctAnswer;    // 정답
    public string[] wrongAnswers;   // 오답 목록
    public float baseWeight = 1f;   // 기본 가중치
    [HideInInspector] public float currentWeight;  // 현재 가중치 (실행 중 변경될 수 있음)
    [HideInInspector] public int sequenceNumber;   // 이미지의 실제 순서를 저장할 필드
}

public class ImageManager : MonoBehaviour
{
    // 싱글톤 패턴을 위한 인스턴스
    public static ImageManager Instance { get; private set; }

    [SerializeField] private Image panelImage;     // 이미지를 표시할 UI Image 컴포넌트
    [SerializeField] private float changeInterval = 2.0f;  // 이미지 변경 간격
    [SerializeField] private List<WeightedImageQuizItem> imageQuizItems = new List<WeightedImageQuizItem>();  // 퀴즈 아이템 리스트

    private List<WeightedImageQuizItem> unusedItems;   // 아직 사용되지 않은 퀴즈 아이템
    private int imagesShownCount = 0;                  // 지금까지 보여준 이미지 수
    private const int ImagesPerRound = 5;              // 라운드당 보여줄 이미지 수
    private List<WeightedImageQuizItem> shownImages = new List<WeightedImageQuizItem>();  // 보여진 이미지들을 순서대로 저장할 리스트

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
        if (imageQuizItems.Count > 0)
        {
            // 사용되지 않은 아이템 리스트 초기화
            unusedItems = new List<WeightedImageQuizItem>(imageQuizItems);
            ResetWeights();
            if (panelImage != null)
            {
                panelImage.gameObject.SetActive(true);
                StartCoroutine(ImageQuizCycle());
            }
            else
            {
                Debug.LogError("Panel Image is not assigned in the Inspector.");
            }
        }
        else
        {
            if (panelImage != null)
            {
                panelImage.gameObject.SetActive(false);
            }
            Debug.LogWarning("Image quiz item list is empty.");
        }
    }

    // 가중치 초기화 메서드
    private void ResetWeights()
    {
        foreach (var item in unusedItems)
        {
            item.currentWeight = item.baseWeight;
        }
    }

    // 이미지 퀴즈 사이클을 관리하는 코루틴
    private IEnumerator ImageQuizCycle()
    {
        while (imagesShownCount < imageQuizItems.Count)
        {
            // 라운드당 이미지 선택
            List<WeightedImageQuizItem> roundItems = GetRandomWeightedItems(ImagesPerRound);
            yield return StartCoroutine(ShowImages(roundItems));

            // 퀴즈 시작
            if (QuizManager.Instance != null)
            {
                QuizManager.Instance.StartQuiz(shownImages);
                yield return new WaitUntil(() => !QuizManager.Instance.IsQuizActive());
            }
            else
            {
                Debug.LogError("QuizManager instance is null.");
                yield break;
            }

            // 라운드가 끝나면 shownImages 리스트를 초기화
            shownImages.Clear();
        }
        Debug.Log("모든 이미지를 보여주었습니다. 프로그램을 종료합니다.");
    }

    // 선택된 이미지들을 보여주는 코루틴
    private IEnumerator ShowImages(List<WeightedImageQuizItem> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (panelImage != null && item.image != null)
            {
                panelImage.sprite = item.image;
                item.sequenceNumber = imagesShownCount + 1; // 실제 순서 저장 (1부터 시작)
                shownImages.Add(item);
            }
            else
            {
                Debug.LogWarning("Image is missing for a quiz item.");
            }
            imagesShownCount++;
            yield return new WaitForSeconds(changeInterval);
            if (imagesShownCount >= imageQuizItems.Count)
            {
                break;
            }
        }
    }

    // 가중치를 고려하여 랜덤하게 아이템을 선택하는 메서드
    private List<WeightedImageQuizItem> GetRandomWeightedItems(int count, List<WeightedImageQuizItem> sourceList = null)
    {
        sourceList = sourceList ?? unusedItems;
        List<WeightedImageQuizItem> result = new List<WeightedImageQuizItem>();
        int itemsToSelect = Mathf.Min(count, sourceList.Count);

        // 정신력에 따라 가중치 조정
        float mentalHealthFactor = MentalHealthManager.Instance.GetMentalHealthPercentage();
        foreach (var item in sourceList)
        {
            item.currentWeight = item.baseWeight * mentalHealthFactor;
        }

        float totalWeight = sourceList.Sum(item => item.currentWeight);
        for (int i = 0; i < itemsToSelect; i++)
        {
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;
            foreach (var item in sourceList)
            {
                cumulativeWeight += item.currentWeight;
                if (randomValue <= cumulativeWeight)
                {
                    result.Add(item);
                    totalWeight -= item.currentWeight;
                    sourceList.Remove(item);
                    break;
                }
            }
        }
        return result;
    }
}
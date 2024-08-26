using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public static ImageManager Instance { get; private set; }

    [SerializeField] private Image panelImage;     // 이미지를 표시할 UI Image 컴포넌트
    [SerializeField] private float changeInterval = 2.0f;  // 이미지 변경 간격
    [SerializeField] private List<WeightedImageQuizItem> imageQuizItems = new List<WeightedImageQuizItem>();  // 퀴즈 아이템 리스트
    [SerializeField] private List<Sprite> tutorialImages = new List<Sprite>();  // 튜토리얼 이미지 리스트
    [SerializeField] private float tutorialImageDuration = 10f;  // 튜토리얼 이미지 표시 시간

    private List<WeightedImageQuizItem> unusedItems;   // 아직 사용되지 않은 퀴즈 아이템
    private int imagesShownCount = 0;                  // 지금까지 보여준 이미지 수
    private const int ImagesPerRound = 5;              // 라운드당 보여줄 이미지 수
    private List<WeightedImageQuizItem> shownImages = new List<WeightedImageQuizItem>();  // 보여진 이미지들을 순서대로 저장할 리스트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
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
            unusedItems = new List<WeightedImageQuizItem>(imageQuizItems);
            ResetWeights();
            if (panelImage != null)
            {
                panelImage.gameObject.SetActive(true);
                StartCoroutine(ImageCycle());
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

    private void ResetWeights()
    {
        foreach (var item in unusedItems)
        {
            item.currentWeight = item.baseWeight;
        }
    }

    private IEnumerator ImageCycle()
    {
        yield return StartCoroutine(ShowTutorialImages());

        while (imagesShownCount < imageQuizItems.Count)
        {
            List<WeightedImageQuizItem> roundItems = GetRandomWeightedItems(ImagesPerRound);
            yield return StartCoroutine(ShowImages(roundItems));

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

            shownImages.Clear();

            // 모든 이미지를 사용했는지 확인
            if (imagesShownCount >= imageQuizItems.Count)
            {
                LoadSuccessEndingScene();
                yield break;
            }
        }
    }

    private IEnumerator ShowTutorialImages()
    {
        foreach (var tutorialImage in tutorialImages)
        {
            if (panelImage != null && tutorialImage != null)
            {
                panelImage.sprite = tutorialImage;
                yield return new WaitForSeconds(tutorialImageDuration);
            }
            else
            {
                Debug.LogWarning("Tutorial image is missing.");
            }
        }
    }

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

    private List<WeightedImageQuizItem> GetRandomWeightedItems(int count, List<WeightedImageQuizItem> sourceList = null)
    {
        sourceList = sourceList ?? unusedItems;
        List<WeightedImageQuizItem> result = new List<WeightedImageQuizItem>();
        int itemsToSelect = Mathf.Min(count, sourceList.Count);

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

    // 성공 엔딩 씬을 로드하는 새로운 메서드
    private void LoadSuccessEndingScene()
    {
        Debug.Log("모든 이미지를 성공적으로 보여주었습니다. 성공 엔딩으로 이동합니다.");
        SceneManager.LoadScene("SuccessEndingScene");
    }

    [ContextMenu("test")]
    public void abcTest()
    {
        SceneManager.LoadScene("InGameScene");
    }
}
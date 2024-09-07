using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 퀴즈 아이템을 위한 클래스
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
    [SerializeField] private List<Sprite> tutorialImages = new List<Sprite>();  // 튜토리얼 이미지 리스트
    [SerializeField] private float tutorialImageDuration = 10f;  // 튜토리얼 이미지 표시 시간
    [SerializeField] private List<WeightedImageQuizItem> tutorialQuizItems = new List<WeightedImageQuizItem>();  // 튜토리얼 퀴즈 아이템 리스트

    private List<WeightedImageQuizItem> unusedItems;   // 아직 사용되지 않은 퀴즈 아이템
    private int imagesShownCount = 0;                  // 지금까지 보여준 이미지 수
    private const int TutorialQuizCount = 5;           // 튜토리얼 퀴즈 개수
    private List<WeightedImageQuizItem> shownImages = new List<WeightedImageQuizItem>();  // 보여진 이미지들을 저장할 리스트

    // 싱글톤 패턴 구현
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 초기화
    private void Start()
    {
        if (imageQuizItems.Count > 0 && tutorialQuizItems.Count > 0)
        {
            // 이미지 퀴즈 아이템을 섞습니다.
            unusedItems = new List<WeightedImageQuizItem>(imageQuizItems);
            ShuffleList(unusedItems);
            // 튜토리얼 퀴즈 아이템도 섞습니다.
            ShuffleList(tutorialQuizItems);
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
            Debug.LogWarning("Image quiz item list or tutorial quiz item list is empty.");
        }
    }

    // 리스트를 섞는 메서드
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // 가중치 초기화
    private void ResetWeights()
    {
        foreach (var item in unusedItems)
        {
            item.currentWeight = item.baseWeight;
        }
    }

    // 이미지 사이클 코루틴
    private IEnumerator ImageCycle()
    {
        // 튜토리얼 시작 전 면역 시작
        MentalHealthManager.Instance.StartImmunity();

        // 튜토리얼 이미지 보여주기
        yield return StartCoroutine(ShowTutorialImages());

        // 튜토리얼 종료 후 면역 해제
        MentalHealthManager.Instance.EndImmunity();

        // 튜토리얼 퀴즈 진행
        int tutorialQuizCount = Mathf.Min(TutorialQuizCount, tutorialQuizItems.Count);
        yield return StartCoroutine(ShowTutorialQuiz(tutorialQuizItems.Take(tutorialQuizCount).ToList()));

        // 본 게임 진행
        while (unusedItems.Count > 0)
        {
            List<WeightedImageQuizItem> roundItems = GetRandomWeightedItems(1);
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
        }

        // 모든 이미지를 사용했을 때
        LoadSuccessEndingScene();
    }

    // 튜토리얼 이미지 표시
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

    // 튜토리얼 퀴즈 진행
    private IEnumerator ShowTutorialQuiz(List<WeightedImageQuizItem> items)
    {
        foreach (var item in items)
        {
            if (panelImage != null && item.image != null)
            {
                panelImage.sprite = item.image;
                yield return new WaitForSeconds(changeInterval);

                shownImages.Clear();
                shownImages.Add(item);

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
            }
            else
            {
                Debug.LogWarning("Image is missing for a tutorial quiz item.");
            }
        }
    }

    // 이미지 표시
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
        }
    }

    // 랜덤 가중치 아이템 선택
    private List<WeightedImageQuizItem> GetRandomWeightedItems(int count, List<WeightedImageQuizItem> sourceList = null)
    {
        sourceList = sourceList ?? unusedItems;
        List<WeightedImageQuizItem> result = new List<WeightedImageQuizItem>();
        int itemsToSelect = Mathf.Min(count, sourceList.Count);

        float mentalHealthFactor = MentalHealthManager.Instance.GetMentalHealthPercentage();

        // 모든 아이템의 가중치를 업데이트하고 총 가중치를 계산
        float totalWeight = 0f;
        foreach (var item in sourceList)
        {
            item.currentWeight = item.baseWeight * mentalHealthFactor;
            totalWeight += item.currentWeight;
        }

        for (int i = 0; i < itemsToSelect; i++)
        {
            if (sourceList.Count == 0) break;

            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;

            for (int j = 0; j < sourceList.Count; j++)
            {
                cumulativeWeight += sourceList[j].currentWeight;
                if (randomValue <= cumulativeWeight)
                {
                    result.Add(sourceList[j]);
                    totalWeight -= sourceList[j].currentWeight;
                    sourceList.RemoveAt(j);
                    break;
                }
            }
        }

        return result;
    }

    // 성공 엔딩 씬을 로드하는 메서드
    private void LoadSuccessEndingScene()
    {
        Debug.Log("모든 이미지를 성공적으로 보여주었습니다. 성공 엔딩으로 이동합니다.");
        SceneManager.LoadScene("SuccessEndingScene");
    }

    // 테스트용 메서드
    [ContextMenu("test")]
    public void abcTest()
    {
        SceneManager.LoadScene("InGameScene");
    }
}
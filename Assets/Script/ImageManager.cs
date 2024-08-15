using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeightedImageQuizItem
{
    public Sprite image;
    public string question;
    public string correctAnswer;
    public string[] wrongAnswers;
    public float baseWeight = 1f;
    [HideInInspector] public float currentWeight;
}

public class ImageManager : MonoBehaviour
{
    public static ImageManager Instance { get; private set; }

    [SerializeField] private Image panelImage;
    [SerializeField] private float changeInterval = 2.0f;
    [SerializeField] private List<WeightedImageQuizItem> imageQuizItems = new List<WeightedImageQuizItem>();

    private List<WeightedImageQuizItem> unusedItems;
    private int imagesShownCount = 0;
    private const int ImagesPerRound = 5;

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
        if (imageQuizItems.Count > 0)
        {
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

    private void ResetWeights()
    {
        foreach (var item in unusedItems)
        {
            item.currentWeight = item.baseWeight;
        }
    }

    private IEnumerator ImageQuizCycle()
    {
        while (imagesShownCount < imageQuizItems.Count)
        {
            List<WeightedImageQuizItem> roundItems = GetRandomWeightedItems(ImagesPerRound);
            yield return StartCoroutine(ShowImages(roundItems));

            List<WeightedImageQuizItem> quizItems = GetRandomWeightedItems(3, roundItems);
            if (QuizManager.Instance != null)
            {
                QuizManager.Instance.StartQuiz(quizItems);
                yield return new WaitUntil(() => !QuizManager.Instance.IsQuizActive());
            }
            else
            {
                Debug.LogError("QuizManager instance is null.");
                yield break;
            }
        }
        Debug.Log("모든 이미지를 보여주었습니다. 프로그램을 종료합니다.");
    }

    private IEnumerator ShowImages(List<WeightedImageQuizItem> items)
    {
        foreach (var item in items)
        {
            if (panelImage != null && item.image != null)
            {
                panelImage.sprite = item.image;
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
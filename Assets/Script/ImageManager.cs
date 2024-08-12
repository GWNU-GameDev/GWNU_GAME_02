using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ImageQuizItem
{
    public Sprite image;
    public string question;
    public string correctAnswer;
    public string[] wrongAnswers;
}

public class ImageManager : MonoBehaviour
{
    public static ImageManager Instance { get; private set; }

    [SerializeField] private Image panelImage;
    [SerializeField] private float changeInterval = 2.0f;
    [SerializeField] private List<ImageQuizItem> imageQuizItems = new List<ImageQuizItem>();

    private List<ImageQuizItem> unusedItems;
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
            unusedItems = new List<ImageQuizItem>(imageQuizItems);
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

    private IEnumerator ImageQuizCycle()
    {
        while (imagesShownCount < imageQuizItems.Count)
        {
            List<ImageQuizItem> roundItems = GetRandomItems(ImagesPerRound);
            yield return StartCoroutine(ShowImages(roundItems));

            List<ImageQuizItem> quizItems = GetRandomItems(3, roundItems);
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

    private IEnumerator ShowImages(List<ImageQuizItem> items)
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

    private List<ImageQuizItem> GetRandomItems(int count, List<ImageQuizItem> sourceList = null)
    {
        sourceList = sourceList ?? unusedItems;
        List<ImageQuizItem> result = new List<ImageQuizItem>();
        int itemsToSelect = Mathf.Min(count, sourceList.Count);
        List<ImageQuizItem> tempList = new List<ImageQuizItem>(sourceList);

        for (int i = 0; i < itemsToSelect; i++)
        {
            int index = UnityEngine.Random.Range(0, tempList.Count);
            result.Add(tempList[index]);
            tempList.RemoveAt(index);
        }
        return result;
    }
}
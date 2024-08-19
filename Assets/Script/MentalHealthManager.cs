using System.Collections;
using UnityEngine;

public class MentalHealthManager : MonoBehaviour
{
    public static MentalHealthManager Instance { get; private set; }
    public int maxMentalHealth = 100;
    public int decreaseRate = 1;
    private int currentMentalHealth;
    public GameObject level2Object;
    public GameObject level3Object;

    private EyesBlinking eyesBlinking;

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

    void Start()
    {
        currentMentalHealth = maxMentalHealth;
        eyesBlinking = FindObjectOfType<EyesBlinking>();
        if (eyesBlinking == null)
        {
            Debug.LogError("EyesBlinking component not found in the scene!");
        }
        UpdateMentalHealthUI();
        UpdateLevelStatus();
    }

    void UpdateMentalHealthUI()
    {
        Debug.Log("Current Mental Health: " + currentMentalHealth);
    }

    public void DecreaseMentalHealth()
    {
        int previousMentalHealth = currentMentalHealth;
        currentMentalHealth -= decreaseRate;
        if (currentMentalHealth < 0)
        {
            currentMentalHealth = 0;
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
    }

    private bool ShouldForceBlinkEyes(int previousHealth, int currentHealth)
    {
        return (previousHealth > 74 && currentHealth == 74) ||
               (previousHealth > 49 && currentHealth <= 49 && previousHealth > 49);
    }

    private IEnumerator ForceBlinkAndUpdateLevel()
    {
        yield return StartCoroutine(eyesBlinking.ForceBlinkCoroutine());
        UpdateLevelStatus();
    }

    public bool IsMentalHealthDepleted()
    {
        return currentMentalHealth <= 0;
    }

    public float GetMentalHealthPercentage()
    {
        return (float)currentMentalHealth / maxMentalHealth;
    }

    private void UpdateLevelStatus()
    {
        if (level2Object != null && level3Object != null)
        {
            if (currentMentalHealth >= 50 && currentMentalHealth <= 74)
            {
                level2Object.SetActive(true);
                level3Object.SetActive(false);
            }
            else if (currentMentalHealth >= 1 && currentMentalHealth <= 49)
            {
                level2Object.SetActive(false);
                level3Object.SetActive(true);
            }
            else
            {
                level2Object.SetActive(false);
                level3Object.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Level2 또는 Level3 오브젝트가 할당되지 않았습니다.");
        }
    }
}
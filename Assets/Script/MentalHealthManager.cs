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

    private float immunityDuration = 50f; // 정신력 감소 면역 시간 (50초)
    private float immunityTimer; // 면역 시간을 추적하는 타이머

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

        immunityTimer = immunityDuration; // 타이머 초기화
        StartCoroutine(ImmunityCountdown());
    }

    void UpdateMentalHealthUI()
    {
        Debug.Log("Current Mental Health: " + currentMentalHealth);
    }

    public void DecreaseMentalHealth()
    {
        if (immunityTimer > 0) // 면역 시간 동안은 정신력 감소 무시
        {
            Debug.Log("Immunity active. Mental health not decreased.");
            return;
        }

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

    private IEnumerator ImmunityCountdown()
    {
        while (immunityTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            immunityTimer--;
            Debug.Log($"Immunity time remaining: {immunityTimer} seconds");
        }
        Debug.Log("Immunity period ended. Mental health can now decrease.");
    }
}
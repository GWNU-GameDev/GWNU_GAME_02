using System.Collections;
using UnityEngine;

public class MentalHealthManager : MonoBehaviour
{
    public static MentalHealthManager Instance { get; private set; }

    public int maxMentalHealth = 100;
    public int decreaseRate = 1;
    private int currentMentalHealth;

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
        UpdateMentalHealthUI();
    }

    void UpdateMentalHealthUI()
    {
        Debug.Log("Current Mental Health: " + currentMentalHealth);
    }

    public void DecreaseMentalHealth()
    {
        currentMentalHealth -= decreaseRate;
        if (currentMentalHealth < 0)
        {
            currentMentalHealth = 0;
        }
        UpdateMentalHealthUI(); 
    }

    public bool IsMentalHealthDepleted()
    {
        return currentMentalHealth <= 0;
    }

    public float GetMentalHealthPercentage()
    {
        return (float)currentMentalHealth / maxMentalHealth;
    }
}
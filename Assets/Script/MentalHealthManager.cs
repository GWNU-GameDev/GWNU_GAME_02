using System.Collections;
using UnityEngine;

public class MentalHealthManager : MonoBehaviour
{
    public int maxMentalHealth = 100; // 최대 정신력
    public int decreaseRate = 1; // 감소율 (초당 감소할 정신력 양)
    private int currentMentalHealth;

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
        UpdateMentalHealthUI(); // 정신력 감소 시마다 업데이트
    }

    public bool IsMentalHealthDepleted()
    {
        return currentMentalHealth <= 0;
    }
}

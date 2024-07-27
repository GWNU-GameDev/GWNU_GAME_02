using System.Collections;
using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    public MentalHealthManager mentalHealthManager;
    public EyesBlinking eyesBlinking; // 눈 깜박임 스크립트 참조
    public string targetTag = "ImagePanel"; // 정신력을 감소시킬 태그
    public float decreaseInterval = 1.0f; // 정신력 감소 간격 (초 단위)

    private Camera cam;
    private bool isDecreasingMentalHealth = false;

    void Start()
    {
        cam = GetComponent<Camera>(); // 현재 오브젝트의 카메라 컴포넌트를 할당
        if (cam == null)
        {
            Debug.LogWarning("Camera component is not attached to this GameObject.");
        }
    }

    void Update()
    {
        CheckForTargets();
    }

    void CheckForTargets()
    {
        if (eyesBlinking != null && eyesBlinking.IsEyesClosed())
        {
            StopDecreaseMentalHealth();
            Debug.Log("Eyes are closed. Mental health will not decrease.");
            return;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit[] hits = Physics.RaycastAll(ray);

        // 레이 시각화
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        bool foundTarget = false;

        foreach (RaycastHit hit in hits)
        {
            string hitTag = hit.collider.tag;
            // Debug.Log("Looking at: " + hitTag);

            if (hitTag == targetTag)
            {
                foundTarget = true;
                break;
            }
        }

        if (foundTarget)
        {
            if (!isDecreasingMentalHealth)
            {
                StartCoroutine(DecreaseMentalHealthOverTime());
            }
        }
        else
        {
            StopDecreaseMentalHealth();
        }
    }

    void StopDecreaseMentalHealth()
    {
        if (isDecreasingMentalHealth)
        {
            StopCoroutine(DecreaseMentalHealthOverTime());
            isDecreasingMentalHealth = false;
        }
    }

    private IEnumerator DecreaseMentalHealthOverTime()
    {
        isDecreasingMentalHealth = true;
        while (isDecreasingMentalHealth)
        {
            mentalHealthManager.DecreaseMentalHealth();
            yield return new WaitForSeconds(decreaseInterval);

            if (mentalHealthManager.IsMentalHealthDepleted())
            {
                // 정신력이 모두 소모된 경우 처리 (예: 게임 오버)
                Debug.Log("Mental Health Depleted!");
                isDecreasingMentalHealth = false;
            }
        }
    }
}

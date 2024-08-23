using UnityEngine;
using System.Collections;

public class CameraBoxCaster : MonoBehaviour
{
    public MentalHealthManager mentalHealthManager;
    public EyesBlinking eyesBlinking;
    public string targetTag = "ImagePanel";
    public float decreaseInterval = 2.0f; // 정신력 감소 간격 (초)

    [SerializeField] private float maxDistance = 20.0f; // BoxCast의 최대 거리
    [SerializeField] private Color rayColor = Color.red; // Gizmo 레이의 색상

    private bool isLookingAtTarget = false; // 현재 타겟을 보고 있는지 여부
    private Coroutine mentalHealthCoroutine; // 정신력 감소 코루틴
    private float lastDecreaseTime; // 마지막으로 정신력이 감소한 시간

    void Update()
    {
        CheckForTargets();
    }

    // 타겟 검사 및 정신력 감소 로직
    void CheckForTargets()
    {
        // 눈이 감겼는지 확인
        if (eyesBlinking != null && eyesBlinking.IsEyesClosed())
        {
            isLookingAtTarget = false;
            return;
        }

        bool currentlyLookingAtTarget = false;

        // BoxCast를 사용하여 타겟 감지
        if (Physics.BoxCast(transform.position, transform.lossyScale / 2.0f, transform.forward, out RaycastHit hit, transform.rotation, maxDistance))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                currentlyLookingAtTarget = true;
            }
        }

        // 타겟을 보는 상태가 변경되었을 때만 코루틴 시작/중지
        if (currentlyLookingAtTarget != isLookingAtTarget)
        {
            isLookingAtTarget = currentlyLookingAtTarget;
            if (isLookingAtTarget)
            {
                StartMentalHealthDecrease();
            }
            else
            {
                StopMentalHealthDecrease();
            }
        }
    }

    // 정신력 감소 시작
    void StartMentalHealthDecrease()
    {
        if (mentalHealthCoroutine == null)
        {
            mentalHealthCoroutine = StartCoroutine(DecreaseMentalHealthOverTime());
        }
    }

    // 정신력 감소 중지
    void StopMentalHealthDecrease()
    {
        if (mentalHealthCoroutine != null)
        {
            StopCoroutine(mentalHealthCoroutine);
            mentalHealthCoroutine = null;
        }
    }

    // 시간에 따른 정신력 감소 코루틴
    private IEnumerator DecreaseMentalHealthOverTime()
    {
        while (true)
        {
            // 마지막 감소 시간으로부터 decreaseInterval 이상 지났는지 확인
            if (Time.time - lastDecreaseTime >= decreaseInterval)
            {
                mentalHealthManager.DecreaseMentalHealth();
                lastDecreaseTime = Time.time;

                if (mentalHealthManager.IsMentalHealthDepleted())
                {
                    Debug.Log("정신력이 모두 소진되었습니다!");
                    StopMentalHealthDecrease();
                    break;
                }
            }
            yield return null; // 매 프레임마다 체크
        }
    }

    // Gizmo를 사용한 BoxCast 시각화 (에디터에서만 동작)
    void OnDrawGizmos()
    {
        Gizmos.color = rayColor;

        if (Physics.BoxCast(transform.position, transform.lossyScale / 2.0f, transform.forward, out RaycastHit hit, transform.rotation, maxDistance))
        {
            // 충돌 지점까지 레이 그리기
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            // 충돌 지점에 와이어 큐브 그리기
            Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, transform.lossyScale);
        }
        else
        {
            // 충돌이 없을 경우 최대 거리까지 레이 그리기
            Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        }
    }
}
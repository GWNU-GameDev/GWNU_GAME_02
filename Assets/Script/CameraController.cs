using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Rotate")]
    public float mouseSpeed = 400f;      // 마우스 감도
    public float rotationAngle = 50.0f;  // 카메라 회전 각도
    public float rotationSpeed = 20.0f;  // 카메라 회전 속도
    public float delayBeforeStart = 5.0f;// 시작 전 대기 시간
    private float yRotation;             // Y축 회전값
    private float xRotation;             // X축 회전값
    private Camera cam;                  // 카메라 컴포넌트
    private bool canRotate = false;      // 회전 가능 여부

    void Start()
    {
        // 마우스 커서를 숨기고 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 카메라 컴포넌트 가져오기
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogWarning("Camera component is not attached to this GameObject.");
        }

        // 주위 둘러보기 시작
        StartCoroutine(StartLookAroundAfterDelay());
    }

    void Update()
    {
        // 회전 가능한 상태일 때만 회전 실행
        if (canRotate)
        {
            Rotate();
        }
    }

    // 카메라 회전 메서드
    void Rotate()
    {
        // 마우스 입력 받기
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;

        // 회전값 계산
        yRotation += mouseX;
        xRotation -= mouseY;

        // 회전 제한 (클램핑)
        yRotation = Mathf.Clamp(yRotation, -60f, 60f);
        xRotation = Mathf.Clamp(xRotation, -30f, 30f);

        // 카메라 회전 적용
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    // 지연 후 주위 둘러보기 시작
    private IEnumerator StartLookAroundAfterDelay()
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(delayBeforeStart);

        // 주위 둘러보기 실행
        yield return StartCoroutine(LookAround());

        // 주위 둘러보기가 끝난 후 마우스 회전 활성화
        canRotate = true;
    }

    // 주위 둘러보기 코루틴
    private IEnumerator LookAround()
    {
        Quaternion originalRotation = transform.rotation;

        // 오른쪽으로 회전
        yield return StartCoroutine(RotateCamera(rotationAngle));

        // 왼쪽으로 회전
        yield return StartCoroutine(RotateCamera(-rotationAngle * 2));

        // 다시 중앙으로 회전
        yield return StartCoroutine(RotateCamera(rotationAngle));
    }

    // 카메라 회전 코루틴
    private IEnumerator RotateCamera(float angle)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, angle, 0);
        float elapsedTime = 0.0f;

        // 부드러운 회전 실행
        while (elapsedTime < Mathf.Abs(angle) / rotationSpeed)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime * rotationSpeed / Mathf.Abs(angle));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 회전 위치 설정
        transform.rotation = endRotation;
    }

    // 마우스 감도 설정 메서드
    public void SetMouseSpeed(float speed)
    {
        mouseSpeed = speed;
    }
}
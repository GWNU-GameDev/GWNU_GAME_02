using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Rotate")]
    public float mouseSpeed = 100f;
    public float rotationAngle = 50.0f;
    public float rotationSpeed = 20.0f;
    public float delayBeforeStart = 5.0f;

    private float yRotation;
    private float xRotation;
    private Camera cam;
    private bool isLookingAround = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // 마우스 커서를 화면 안에서 고정
        Cursor.visible = false;                     // 마우스 커서를 보이지 않도록 설정

        cam = GetComponent<Camera>();               // 현재 오브젝트의 카메라 컴포넌트를 할당
        if (cam == null)
        {
            Debug.LogWarning("Camera component is not attached to this GameObject.");
        }

        StartCoroutine(StartLookAroundAfterDelay());
    }

    void Update()
    {
        if (!isLookingAround)
        {
            Rotate();
        }
    }
    
    void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;

        yRotation += mouseX;    // 마우스 X축 입력에 따라 수평 회전 값을 조정
        xRotation -= mouseY;    // 마우스 Y축 입력에 따라 수직 회전 값을 조정

        yRotation = Mathf.Clamp(yRotation, -60f, 60f);  // 수평 회전 값을 -60도에서 60도 사이로 제한
        xRotation = Mathf.Clamp(xRotation, -30f, 30f);  // 수직 회전 값을 -30도에서 30도 사이로 제한

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0); // 현재 오브젝트의 회전을 조절
    }

    private IEnumerator StartLookAroundAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        yield return StartCoroutine(LookAround());
    }

    private IEnumerator LookAround()
    {
        isLookingAround = true;
        Quaternion originalRotation = transform.rotation;

        // Rotate to the right
        yield return StartCoroutine(RotateCamera(rotationAngle));

        // Rotate back to original
        yield return StartCoroutine(RotateCamera(-rotationAngle * 2));

        // Rotate back to center
        yield return StartCoroutine(RotateCamera(rotationAngle));

        isLookingAround = false;
    }

    private IEnumerator RotateCamera(float angle)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, angle, 0);
        float elapsedTime = 0.0f;

        while (elapsedTime < Mathf.Abs(angle) / rotationSpeed)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime * rotationSpeed / Mathf.Abs(angle));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
    }
}

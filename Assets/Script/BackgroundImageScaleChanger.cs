using UnityEngine;
using System.Collections;

public class BackgroundImageScaleChanger : MonoBehaviour
{
    public float delay = 50f; // 대기 시간 (초)
    public float targetScale = 0.7f; // 목표 스케일 값

    void Start()
    {
        // 50초 후에 ChangeScale 메서드를 실행합니다.
        StartCoroutine(ChangeScaleAfterDelay());
    }

    IEnumerator ChangeScaleAfterDelay()
    {
        // 지정된 시간(delay)만큼 대기합니다.
        yield return new WaitForSeconds(delay);

        // 오브젝트의 스케일을 변경합니다.
        transform.localScale = new Vector3(targetScale, targetScale, targetScale);

        Debug.Log("BackgroundImage의 스케일이 " + targetScale + "로 변경되었습니다.");
    }
}
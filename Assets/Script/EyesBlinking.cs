using System.Collections;
using UnityEngine;

public class EyesBlinking : MonoBehaviour
{
    public Animator animator;
    private bool eyesClosed = false;

    void Start()
    {
        StartCoroutine(BlinkAtStart());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleEyes();
        }
    }

    void ToggleEyes()
    {
        eyesClosed = !eyesClosed;

        // 눈을 감거나 뜨는 애니메이션 상태를 전환합니다.
        if (eyesClosed)
        {
            // 눈을 감는 애니메이션을 재생합니다.
            animator.SetTrigger("EyesClose");
        }
        else
        {
            // 눈을 뜨는 애니메이션을 재생합니다.
            animator.SetTrigger("EyesOpen");
        }
    }

    IEnumerator BlinkAtStart()
    {
        // 눈을 감는 애니메이션을 재생합니다.
        animator.SetTrigger("EyesClose");
        yield return new WaitForSeconds(1.5f); // 눈을 감은 상태로 대기 (1.5초 대기)
        // 눈을 뜨는 애니메이션을 재생합니다.
        animator.SetTrigger("EyesOpen");

        yield return new WaitForSeconds(3.0f);
        // 눈을 감는 애니메이션을 재생합니다.
        animator.SetTrigger("EyesClose");
        yield return new WaitForSeconds(1.5f); // 눈을 감은 상태로 대기 (1.5초 대기)
        // 눈을 뜨는 애니메이션을 재생합니다.
        animator.SetTrigger("EyesOpen");
    }

    public bool IsEyesClosed()
    {
        return eyesClosed;
    }
}

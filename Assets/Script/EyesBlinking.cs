using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesBlinking : MonoBehaviour
{
    public Animator animator;
    private bool eyesClosed = false;

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
}


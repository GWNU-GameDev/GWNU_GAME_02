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
        if (eyesClosed)
        {
            CloseEyes();
        }
        else
        {
            OpenEyes();
        }
    }

    void CloseEyes()
    {
        eyesClosed = true;
        animator.SetTrigger("EyesClose");
    }

    void OpenEyes()
    {
        eyesClosed = false;
        animator.SetTrigger("EyesOpen");
    }

    IEnumerator BlinkAtStart()
    {
        // 첫 번째 깜빡임
        CloseEyes();
        yield return new WaitForSeconds(1.5f);
        OpenEyes();
        yield return new WaitForSeconds(3.0f);

        // 두 번째 깜빡임
        CloseEyes();
        yield return new WaitForSeconds(1.5f);
        OpenEyes();
    }

    public IEnumerator ForceBlinkCoroutine()
    {
        CloseEyes();
        yield return new WaitForSeconds(1.0f);
        OpenEyes();
        
    }

    public bool IsEyesClosed()
    {
        return eyesClosed;
    }
}
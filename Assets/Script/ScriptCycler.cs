using System.Collections;
using UnityEngine;

public class EffectSequencer : MonoBehaviour
{
    public MonoBehaviour chzzkScript;
    public MonoBehaviour colorChangeScript;
    public MonoBehaviour noiseScript;

    private void Start()
    {
        StartCoroutine(EffectSequence());
    }

    private IEnumerator EffectSequence()
    {
        yield return new WaitForSeconds(30f);

        while (true)
        {
            // 일반 상태 유지 (15초)
            yield return new WaitForSeconds(15f);

            // 치지직 효과: 0.5초 켜고 끄기를 2번 반복
            yield return StartCoroutine(ApplyChzzkEffect());

            // 대기 (10초)
            yield return new WaitForSeconds(10f);

            // 색 변환 효과: 5초간 유지
            yield return StartCoroutine(ApplyColorChangeEffect());

            // 대기 (15초)
            yield return new WaitForSeconds(15f);

            // 노이즈 효과: 3초간 유지
            yield return StartCoroutine(ApplyNoiseEffect());

            // 대기 (20초)
            yield return new WaitForSeconds(20f);
        }
    }

    private IEnumerator ApplyChzzkEffect()
    {
        for (int i = 0; i < 2; i++)
        {
            chzzkScript.enabled = true;
            yield return new WaitForSeconds(0.25f);
            chzzkScript.enabled = false;
            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator ApplyColorChangeEffect()
    {
        colorChangeScript.enabled = true;
        yield return new WaitForSeconds(5f);
        colorChangeScript.enabled = false;
    }

    private IEnumerator ApplyNoiseEffect()
    {
        noiseScript.enabled = true;
        yield return new WaitForSeconds(3f);
        noiseScript.enabled = false;
    }
}
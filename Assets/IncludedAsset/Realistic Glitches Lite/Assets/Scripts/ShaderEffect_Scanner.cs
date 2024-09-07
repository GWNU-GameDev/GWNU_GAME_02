using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_Scanner : MonoBehaviour
{
    public float area = 0.5f;

    private Material material_a;
    private Material material_b;

    private void OnEnable()
    {
        // 머티리얼이 null인 경우에만 새로 생성
        if (material_a == null)
            material_a = new Material(Shader.Find("Hidden/Shift"));
        if (material_b == null)
            material_b = new Material(Shader.Find("Hidden/Shift"));
    }

    private void OnDisable()
    {
        // 스크립트가 비활성화될 때 머티리얼 해제
        DestroyImmediate(material_a);
        DestroyImmediate(material_b);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material_a == null || material_b == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        material_a.SetFloat("_ValueY", area);
        material_b.SetFloat("_ValueY", -area);

        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

        Graphics.Blit(source, temp, material_a);
        Graphics.Blit(temp, destination, material_b);

        RenderTexture.ReleaseTemporary(temp);
    }
}
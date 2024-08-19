using UnityEngine;
using System.Collections;

public class LightFadeAndMaterialChange : MonoBehaviour
{
    public Light pointLight;
    public Renderer meshRenderer;
    public Material offMaterial;
    public float fadeDuration = 50f;

    private float initialRange;

    void Start()
    {
        if (pointLight == null)
        {
            pointLight = GetComponentInChildren<Light>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<Renderer>();
        }

        if (pointLight != null)
        {
            initialRange = pointLight.range;
            StartCoroutine(FadeLightAndChangeMaterial());
        }
        else
        {
            Debug.LogError("Point Light component not found!");
        }
    }

    IEnumerator FadeLightAndChangeMaterial()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            pointLight.range = Mathf.Lerp(initialRange, 0f, t);
            yield return null;
        }

        pointLight.range = 0f;

        if (meshRenderer != null && offMaterial != null)
        {
            Material[] materials = meshRenderer.materials;
            if (materials.Length > 0)
            {
                materials[0] = offMaterial;
                meshRenderer.materials = materials;
            }
            else
            {
                Debug.LogWarning("No materials found on the Mesh Renderer!");
            }
        }
        else
        {
            Debug.LogError("Mesh Renderer or OFF material not assigned!");
        }
    }
}
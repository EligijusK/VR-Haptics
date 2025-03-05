using System.Collections;
using UnityEngine;

public class MaterialFader : MonoBehaviour
{
    [SerializeField] private GameObject object1;
    [SerializeField] private GameObject object2;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private MeshCollider nonPaintableMeshCollider;

    public void PerformFade()
    {
        if (object1 == null || object2 == null)
        {
            Debug.LogError("GameObjects are not assigned!");
            return;
        }

        // Grab the instance of the material on object1’s renderer
        Material instancedMat = object1.GetComponent<Renderer>().material;
        if (instancedMat == null)
        {
            Debug.LogError("No Material found on object1’s Renderer!");
            return;
        }

        // If you’re using Built-in Standard:
        SetBuiltInStandardMaterialToFade(instancedMat);

        // If you’re using URP, swap the above line with:
        // SetURPMaterialToFade(instancedMat);

        StartCoroutine(FadeOutMaterial(instancedMat, fadeDuration));
    }

    private void SetBuiltInStandardMaterialToFade(Material mat)
    {
        mat.SetFloat("_Mode", 2f); // Fade
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);

        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        mat.renderQueue = 3000; // Transparent queue
    }

    // Optional URP version:
    private void SetURPMaterialToFade(Material mat)
    {
        mat.SetFloat("_Surface", 1);  // 1 = Transparent
        mat.SetFloat("_AlphaClip", 0);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = 3000;
        // Use _BaseColor for fading rather than _Color
    }

    private IEnumerator FadeOutMaterial(Material mat, float duration)
    {
        // For Built-in Standard, we can read & write _Color
        // For URP, you’d do mat.GetColor("_BaseColor") / mat.SetColor("_BaseColor", ...)

        Color startColor = mat.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's fully transparent
        mat.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Enable the collider
        if (nonPaintableMeshCollider != null)
        {
            nonPaintableMeshCollider.enabled = true;
        }

        // Finally disable objects
        object1.SetActive(false);
        object2.SetActive(false);
    }
}

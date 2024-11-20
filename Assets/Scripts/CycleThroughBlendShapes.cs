using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleThroughBlendShapes : MonoBehaviour
{
    public bool smallCloth = true;
    public Transform destinationTransform;
    public float animationDuration = 5.0f;
    public SkinnedMeshRenderer meshRenderer;
    private int blendShapeCount;

    void Start()
    {
        blendShapeCount = meshRenderer.sharedMesh.blendShapeCount;
        ResetBlendShapes();
    }

    public void ResetBlendShapes()
    {
        for (int i = 0; i < blendShapeCount; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, 0);
        }
    }

    public IEnumerator PlayBlendShapeAnimation()
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float normalizedTime = elapsedTime / animationDuration;

            float blendShapeIndex = normalizedTime * (blendShapeCount - 1);
            int lowerIndex = Mathf.Clamp(Mathf.FloorToInt(blendShapeIndex), 0, blendShapeCount - 1);
            int upperIndex = Mathf.Clamp(lowerIndex + 1, 0, blendShapeCount - 1);
            float weightFraction = blendShapeIndex - lowerIndex;

            for (int i = 0; i < blendShapeCount; i++)
            {
                meshRenderer.SetBlendShapeWeight(i, 0);
            }

            if (lowerIndex >= 0 && lowerIndex < blendShapeCount)
            {
                meshRenderer.SetBlendShapeWeight(lowerIndex, (1 - weightFraction) * 100);
            }

            if (upperIndex >= 0 && upperIndex < blendShapeCount)
            {
                meshRenderer.SetBlendShapeWeight(upperIndex, weightFraction * 100);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        for (int i = 0; i < blendShapeCount - 1; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, 0);
        }
        meshRenderer.SetBlendShapeWeight(blendShapeCount - 1, 100);

        enabled = false;
    }

}

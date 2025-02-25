using UnityEngine;
using System.Collections;

public class AccumulateBlendShapes : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;
    [Tooltip("Total duration for animating through all blend shapes.")]
    public float totalAnimationDuration = 5f;

    private int blendShapeCount;

    void Start()
    {
        // Count how many blend shapes are on this mesh
        blendShapeCount = meshRenderer.sharedMesh.blendShapeCount;

        // Reset all to 0% initially (optional)
        for (int i = 0; i < blendShapeCount; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, 0f);
        }

        // Start our animation routine
        StartCoroutine(PlayBlendShapeAnimation());
    }

    private IEnumerator PlayBlendShapeAnimation()
    {
        // We'll divide the total time among each blend shape
        float durationPerShape = totalAnimationDuration / blendShapeCount;

        // Iterate over each blend shape
        for (int i = 0; i < blendShapeCount; i++)
        {
            float startTime = Time.time;
            float endTime = startTime + durationPerShape;

            // Animate this blend shape from 0 → 100 over durationPerShape
            while (Time.time < endTime)
            {
                float t = Mathf.InverseLerp(startTime, endTime, Time.time);
                float weight = Mathf.Lerp(0, 100f, t);
                meshRenderer.SetBlendShapeWeight(i, weight);

                // Keep previously animated shapes at 100% — no reset!
                yield return null;
            }

            // Ensure this shape is exactly 100% at the end
            meshRenderer.SetBlendShapeWeight(i, 100f);
        }

        // Now all blend shapes are at 100%
        Debug.Log("All blend shapes reached 100%!");
    }
}
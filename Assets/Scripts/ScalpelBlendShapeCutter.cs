using UnityEngine;

[RequireComponent(typeof(Collider))] // so we can detect scalpel triggers if needed
public class ScalpelBlendShapeCutter : MonoBehaviour
{
    [Header("Blend Shape Setup")]
    public SkinnedMeshRenderer meshRenderer;   // The skinned mesh with blend shapes
    private int blendShapeCount;

    [Header("Cut Progress Setup")]
    public Transform cutStart;                 // Start of the cut path
    public Transform cutEnd;                   // End of the cut path
    [Range(0f, 1f)] 
    public float cutProgress = 0f;             // 0=uncut, 1=fully cut

    [Header("Scalpel Setup")]
    public Transform scalpel;                  // The scalpel transform
    public bool isScalpelCutting = false;      // Are we currently "cutting" or not?

    void Start()
    {
        if (!meshRenderer)
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        // Count how many blend shapes are in the mesh
        blendShapeCount = meshRenderer.sharedMesh.blendShapeCount;

        // (Optional) Clear all blend shapes at start
        ResetAllBlendShapes();
    }

    void Update()
    {
        if (scalpel == null || cutStart == null || cutEnd == null) 
            return;

        // If the scalpel is "cutting" (e.g., inside trigger), we measure progress
        if (isScalpelCutting)
        {
            // 1) Compute a new progress ratio (0..1) based on scalpel position
            float newProgress = ComputeProgressAlongCutLine();
            
            // 2) Only advance if the scalpel progressed forward
            //    That is, newProgress must exceed our current cutProgress
            if (newProgress > cutProgress)
            {
                cutProgress = newProgress;
                ApplyBlendShapeProgress(cutProgress);
            }
        }
    }

    // Resets all blend shapes to 0 weight
    public void ResetAllBlendShapes()
    {
        for (int i = 0; i < blendShapeCount; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, 0);
        }
        cutProgress = 0f;
    }

    // Apply the current progress to the blend shapes, 
    // assuming they form a linear progression from shape 0..N-1
    private void ApplyBlendShapeProgress(float progress01)
    {
        // Example approach:
        //   We interpret progress as an index from 0..(blendShapeCount-1)
        //   Then do an interpolation between two adjacent shapes.
        float totalShapesF = blendShapeCount - 1; // number of shape transitions
        float floatIndex = progress01 * totalShapesF; // range: 0..(N-1)

        int lowerIdx = Mathf.FloorToInt(floatIndex);
        int upperIdx = Mathf.Clamp(lowerIdx + 1, 0, blendShapeCount - 1);

        float frac = floatIndex - lowerIdx; // how far between lowerIdx and upperIdx

        // Clear all shapes first (or track them and only clear the relevant ones)
        for (int i = 0; i < blendShapeCount; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, 0);
        }

        // Blend between lower and upper
        if (lowerIdx >= 0 && lowerIdx < blendShapeCount)
        {
            meshRenderer.SetBlendShapeWeight(lowerIdx, (1.0f - frac) * 100f);
        }
        if (upperIdx >= 0 && upperIdx < blendShapeCount)
        {
            meshRenderer.SetBlendShapeWeight(upperIdx, frac * 100f);
        }
    }

    // This computes the scalpel's "fraction" along the line from cutStart to cutEnd
    // 0 = at cutStart, 1 = at cutEnd
    private float ComputeProgressAlongCutLine()
    {
        Vector3 startPos = cutStart.position;
        Vector3 endPos = cutEnd.position;
        Vector3 scalpelPos = scalpel.position;

        // Project the scalpel's position onto the segment [startPos..endPos]
        Vector3 direction = endPos - startPos;
        float lineLengthSqr = direction.sqrMagnitude;
        if (lineLengthSqr < 0.0001f)
            return 0f; // safety check if start/end are almost the same

        float t = Vector3.Dot(scalpelPos - startPos, direction) / lineLengthSqr;
        float clampedT = Mathf.Clamp01(t); // ensure 0..1

        return clampedT;
    }

    // Example trigger logic:
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Scalpel"))
        {
            isScalpelCutting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Scalpel"))
        {
            isScalpelCutting = false;
        }
    }
}

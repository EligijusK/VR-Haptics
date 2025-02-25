using UnityEngine;
using UnityEngine.Formats.Alembic.Importer; // For AlembicStreamPlayer

public class AccumulateBlendShapesWhileCutting : MonoBehaviour
{
    [Header("References")]
    public SkinnedMeshRenderer meshRenderer;
    public Transform scalpel;  // For tracking movement

    [Header("Alembic #1")]
    public AlembicStreamPlayer liquidAlembic1; // Assign in Inspector
    public float startFraction1 = 0.3f; // Cut fraction at which this alembic begins
    public float alembicPlaybackSpeed1 = 1f;
    private bool hasStartedAlembic1 = false;

    [Header("Alembic #2")]
    public AlembicStreamPlayer liquidAlembic2; 
    public float startFraction2 = 0.5f; 
    public float alembicPlaybackSpeed2 = 1f;
    private bool hasStartedAlembic2 = false;

    [Header("Blend Shapes")]
    [Tooltip("Max distance the scalpel must move to reach 100% blend")]
    public float maxCutDistance = 0.5f;

    private int blendShapeCount;
    private float currentCutFraction = 0f;
    private bool isCuttingActive = false;
    
    private float accumulatedDistance = 0f;
    private Vector3 lastScalpelPosition;

    void Start()
    {
        if (meshRenderer == null)
        {
            Debug.LogError("No SkinnedMeshRenderer assigned!");
            enabled = false;
            return;
        }

        blendShapeCount = meshRenderer.sharedMesh.blendShapeCount;

        // Reset all blend shapes
        for (int i = 0; i < blendShapeCount; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, 0f);
        }

        // Initialize scalpel tracking
        if (scalpel != null)
        {
            lastScalpelPosition = scalpel.position;
        }

        // Reset each Alembic to time=0
        if (liquidAlembic1 != null)
            liquidAlembic1.CurrentTime = 0f;

        if (liquidAlembic2 != null)
            liquidAlembic2.CurrentTime = 0f;
    }

    void Update()
    {
        if (!isCuttingActive)
        {
            // Still update last position so big jumps won't cause huge increments
            if (scalpel != null) lastScalpelPosition = scalpel.position;
            return;
        }

        if (scalpel != null)
        {
            // 1) Distance traveled this frame
            Vector3 currentPos = scalpel.position;
            float movementThisFrame = Vector3.Distance(currentPos, lastScalpelPosition);

            // 2) Accumulate it
            accumulatedDistance += movementThisFrame;

            // 3) Compute fraction
            currentCutFraction = accumulatedDistance / maxCutDistance;
            currentCutFraction = Mathf.Clamp01(currentCutFraction);

            // 4) Update the blend shapes
            UpdateBlendShapes(currentCutFraction);

            // 5) Check if alembic #1 should start
            if (!hasStartedAlembic1 && currentCutFraction >= startFraction1 && liquidAlembic1 != null)
            {
                hasStartedAlembic1 = true;
                Debug.Log("Alembic #1 started at fraction " + startFraction1);
            }

            // 6) Check if alembic #2 should start
            if (!hasStartedAlembic2 && currentCutFraction >= startFraction2 && liquidAlembic2 != null)
            {
                hasStartedAlembic2 = true;
                Debug.Log("Alembic #2 started at fraction " + startFraction2);
            }

            lastScalpelPosition = currentPos;
        }
    }

    void LateUpdate()
    {
        // If Alembic #1 started, increment its CurrentTime
        if (hasStartedAlembic1 && liquidAlembic1 != null)
        {
            float newTime = liquidAlembic1.CurrentTime + Time.deltaTime * alembicPlaybackSpeed1;
            float duration = liquidAlembic1.Duration;
            if (newTime > duration) newTime = duration;
            liquidAlembic1.CurrentTime = newTime;
        }

        // If Alembic #2 started, increment its CurrentTime
        if (hasStartedAlembic2 && liquidAlembic2 != null)
        {
            float newTime = liquidAlembic2.CurrentTime + Time.deltaTime * alembicPlaybackSpeed2;
            float duration = liquidAlembic2.Duration;
            if (newTime > duration) newTime = duration;
            liquidAlembic2.CurrentTime = newTime;
        }
    }

    // This function is same as before
    private void UpdateBlendShapes(float fraction)
    {
        float totalBlendIndex = fraction * (blendShapeCount - 1);
        int lowerIndex = Mathf.FloorToInt(totalBlendIndex);
        int upperIndex = Mathf.Min(lowerIndex + 1, blendShapeCount - 1);
        float weightFraction = totalBlendIndex - lowerIndex;

        // Clear weights
        for (int i = 0; i < blendShapeCount; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, 0f);
        }

        // Full 100% for all shapes below lowerIndex
        for (int i = 0; i < lowerIndex; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, 100f);
        }

        // Partial for 'lowerIndex'
        meshRenderer.SetBlendShapeWeight(lowerIndex, (1f - weightFraction) * 100f);

        // Partial for 'upperIndex'
        if (upperIndex != lowerIndex)
        {
            meshRenderer.SetBlendShapeWeight(upperIndex, weightFraction * 100f);
        }
    }

    public void SetCuttingActive(bool active)
    {
        isCuttingActive = active;
    }
}

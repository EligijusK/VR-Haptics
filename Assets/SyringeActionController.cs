using System.Collections;
using UnityEngine;

public class SyringeActionController : MonoBehaviour
{
    [Header("Plunger References")]
    [SerializeField] private Transform plunger;            
    [SerializeField] private Transform extendedPosition;    
    [SerializeField] private Transform originalPosition;    

    [Header("Timing")]
    [SerializeField] private float interpolationDuration = 0.5f; 

    [Header("Wound Material")]
    [SerializeField] private Material woundMaterial; 
    [SerializeField] private float fadeDuration = 1f;

    [Header("Drip Object")]
    [SerializeField] private GameObject drip;

    private bool isExtended = false; 
    private bool isMoving = false;    
    private bool inWound = false;

    /// <summary>
    /// How many times we've cleaned the wound so far.
    /// 0 = fully opaque
    /// 1 = half transparent
    /// 2 = fully transparent (drip is disabled)
    /// </summary>
    private int timesCleaned = 0;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // At start, wound is fully opaque.
        SetMaterialAlpha(1f);
        // Ensure drip is initially active.
        drip.SetActive(true);
    }

    public void TogglePlunger()
    {
        if (isMoving) return;
        StartCoroutine(InterpolatePlunger(!isExtended));
    }

    private IEnumerator InterpolatePlunger(bool toExtended)
    {
        isMoving = true;

        Vector3 startLocalPos = plunger.localPosition;
        Quaternion startLocalRot = plunger.localRotation;

        float elapsedTime = 0f;
        while (elapsedTime < interpolationDuration)
        {
            float t = elapsedTime / interpolationDuration;

            Vector3 targetLocalPos = toExtended
                ? extendedPosition.localPosition
                : originalPosition.localPosition;
            Quaternion targetLocalRot = toExtended
                ? extendedPosition.localRotation
                : originalPosition.localRotation;

            plunger.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            plunger.localRotation = Quaternion.Slerp(startLocalRot, targetLocalRot, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        plunger.localPosition = toExtended
            ? extendedPosition.localPosition
            : originalPosition.localPosition;
        plunger.localRotation = toExtended
            ? extendedPosition.localRotation
            : originalPosition.localRotation;

        isExtended = toExtended;
        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wound"))
        {
            inWound = true;

            // If the plunger is retracted => initiate cleaning
            if (!isExtended)
            {
                CleanWound();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wound"))
        {
            inWound = false;
        }
    }

    private void CleanWound()
    {
        // If we've already done 2 or more cleans, do nothing
        // (the wound is already fully transparent & drip should be disabled)
        if (timesCleaned >= 2) return;

        timesCleaned++;
        Debug.Log("cleaned wound");

        float newAlpha = (timesCleaned == 1) ? 0.5f : 0f;

        // If a fade is ongoing, stop it before starting a new one
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeMaterialAlpha(newAlpha, fadeDuration));
    }

    private IEnumerator FadeMaterialAlpha(float targetAlpha, float fadeTime)
    {
        float startAlpha = woundMaterial.color.a;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTime);

            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            SetMaterialAlpha(alpha);

            yield return null;
        }

        // Snap to final alpha
        SetMaterialAlpha(targetAlpha);
        fadeCoroutine = null;

        // If we've just completed the second cleaning, disable the drip
        if (timesCleaned >= 2)
        {
            drip.SetActive(false);
        }
    }

    private void SetMaterialAlpha(float alpha)
    {
        if (woundMaterial == null) return;

        Color c = woundMaterial.color;
        c.a = alpha;
        woundMaterial.color = c;
    }
}

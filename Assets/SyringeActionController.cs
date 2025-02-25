using System.Collections;
using UnityEngine;

public class SyringeActionController : MonoBehaviour
{
    [SerializeField] private Transform plunger;
    [SerializeField] private Transform extendedPosition;
    [SerializeField] private Transform originalPosition;
    [SerializeField] private float interpolationDuration = 0.5f;
    private bool isExtended = false;
    private bool isMoving = false;

    public void TogglePlunger()
    {
        if (!isMoving)
        {
            StartCoroutine(InterpolatePlunger());
            isExtended = !isExtended;
        }
    }

    private IEnumerator InterpolatePlunger()
    {
        isMoving = true;
        Vector3 startPosition = plunger.position;
        Quaternion startRotation = plunger.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < interpolationDuration)
        {
            float t = elapsedTime / interpolationDuration;
            Vector3 targetPosition = isExtended ? originalPosition.position : extendedPosition.position;
            Quaternion targetRotation = isExtended ? originalPosition.rotation : extendedPosition.rotation;

            plunger.position = Vector3.Lerp(startPosition, targetPosition, t);
            plunger.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        plunger.position = isExtended ? originalPosition.position : extendedPosition.position;
        plunger.rotation = isExtended ? originalPosition.rotation : extendedPosition.rotation;
        isMoving = false;
    }
}
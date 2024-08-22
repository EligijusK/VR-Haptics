using System.Collections;
using UnityEngine;

public class PouringAnimation : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isMoving = false;

    private Rigidbody _rigidbody;

    void Start()
    {
        // Assuming the object has a Rigidbody component
        _rigidbody = GetComponent<Rigidbody>();
    }

    public IEnumerator MoveToPouringPosition(GameObject targetObject, float moveDuration, float pourRotationDuration, float holdDuration)
    {
        if (isMoving) yield break; // Prevents overlapping animations

        isMoving = true;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        // Disable Rigidbody physics during the animation
        if (_rigidbody != null)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }

        Vector3 targetPosition = targetObject.transform.position;

        // Move to the target position
        yield return StartCoroutine(MoveToPosition(targetPosition, moveDuration));

        // Hold at the target position
        yield return new WaitForSeconds(holdDuration);

        // Rotate 45 degrees (e.g., around the Z-axis for pouring)
        yield return StartCoroutine(RotateToAngle(Quaternion.Euler(90f, 45f, 0f), pourRotationDuration));

        // Hold the rotated position
        yield return new WaitForSeconds(holdDuration);

        // Rotate back to the original rotation
        yield return StartCoroutine(RotateToAngle(originalRotation, pourRotationDuration));

        // Move back to the original position
        yield return StartCoroutine(MoveToPosition(originalPosition, moveDuration));

        // Enable Rigidbody physics after the animation
        if (_rigidbody != null)
        {
            _rigidbody.constraints = RigidbodyConstraints.None;
        }

        isMoving = false;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }

    private IEnumerator RotateToAngle(Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}

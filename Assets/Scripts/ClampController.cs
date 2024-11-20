using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClampController : MonoBehaviour
{
    [SerializeField] private float interpolationDuration = 1.0f;
    [SerializeField] private XRSimpleInteractable _simpleInteractable;
    [SerializeField] private Rigidbody _rigidbody;
    private Transform targetCorner; 
    private bool isClamping = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider is part of a CornerController
        CornerController cornerController = other.GetComponentInParent<CornerController>();

        if (cornerController != null && !isClamping)
        {
            _simpleInteractable.enabled = false;
            _rigidbody.isKinematic = true;
            targetCorner = cornerController.GetTargetTransform();
            StartCoroutine(InterpolateToCorner());
        }
    }

    private IEnumerator InterpolateToCorner()
    {
        isClamping = true;

        // Initial transform values
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // Target transform values
        Vector3 targetPosition = targetCorner.position;
        Quaternion targetRotation = targetCorner.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < interpolationDuration)
        {
            float t = elapsedTime / interpolationDuration;

            // Interpolate position and rotation
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final values match the target
        transform.position = targetPosition;
        transform.rotation = targetRotation;

        isClamping = false;
    }
}
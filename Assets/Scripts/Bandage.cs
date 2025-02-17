using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Bandage : MonoBehaviour
{
    [SerializeField] private float interpolationDuration = 1.0f;
    [SerializeField] private XRGrabInteractable _grabInteractable;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private CycleThroughBlendShapes bandage;
    private void OnTriggerEnter(Collider other)
    {
        CornerController bandagePositionCollider = other.GetComponentInParent<CornerController>();
        if (bandagePositionCollider != null && !bandagePositionCollider.isForClamps)
        {
            _grabInteractable.enabled = false;
            _rigidbody.isKinematic = true;
            StartCoroutine(InterpolateToCorner());
        }
    }

    private IEnumerator InterpolateToCorner()
    {

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // Target transform values
        Vector3 targetPosition = targetTransform.position;
        Quaternion targetRotation = targetTransform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < interpolationDuration)
        {
            float t = elapsedTime / interpolationDuration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
        yield return StartCoroutine(bandage.PlayBlendShapeAnimation());

    }
}


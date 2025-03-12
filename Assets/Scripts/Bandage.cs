using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class Bandage : MonoBehaviour
{
    [SerializeField] private float interpolationDuration = 1.0f;
    [SerializeField] private XRGrabInteractable _grabInteractable;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private CycleThroughBlendShapes bandage;
    [SerializeField] private GameObject wound; 

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

        Coroutine bandageCoroutine = StartCoroutine(bandage.PlayBlendShapeAnimation());
        Coroutine woundCoroutine = StartCoroutine(LowerWound());

        yield return bandageCoroutine;
        yield return woundCoroutine;
    
        // Wait for 5 seconds before proceeding with the if statement
        yield return new WaitForSeconds(5f);

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
            Debug.Log("Veikia galimai");
        }
        SceneManager.LoadScene("ExitScene");
    }

    private IEnumerator LowerWound()
    {
        if (wound == null)
        {
            Debug.LogError("Wound GameObject is not assigned.");
            yield break;
        }

        Vector3 startPosition = wound.transform.position;
        Vector3 targetPosition = startPosition + Vector3.down;

        float elapsedTime = 0f;
        float animationDuration = 1f; 

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            wound.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        wound.transform.position = targetPosition;
    }
}

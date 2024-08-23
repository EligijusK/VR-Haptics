using System.Collections;
using UnityEngine;

public class PouringAnimation : MonoBehaviour
{
    [SerializeField] private GameObject particleSystem;
    [SerializeField] private GameObject capObject;
    [SerializeField] private float capMoveUpDistance = 0.2f;
    [SerializeField] private float capMoveSideDistance = 0.1f;
    [SerializeField] private float capMoveDuration = 0.5f;
    [SerializeField] private GameObject liquidMesh;
    [SerializeField] private float fillDuration = 3.0f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform capOriginalParent;
    private bool isMoving = false;
    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        capOriginalParent = capObject.transform.parent;
        liquidMesh.SetActive(false);
    }

    public IEnumerator PerformPouringAnimation(GameObject targetObject, float moveDuration, float pourRotationDuration, float holdDuration)
    {
        if (isMoving) yield break;

        isMoving = true;
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        SetRigidbodyConstraints(true);
        yield return MoveCap(new Vector3(capMoveSideDistance, capMoveUpDistance, 0f), capMoveDuration);
        capObject.transform.parent = null;
        Vector3 targetPosition = targetObject.transform.position;
        yield return MoveToPosition(targetPosition, moveDuration);
        yield return new WaitForSeconds(holdDuration);
        yield return StartPouringAndFilling(pourRotationDuration);
        yield return new WaitForSeconds(holdDuration);
        yield return RotateToAngle(originalRotation, pourRotationDuration);
        yield return MoveToPosition(originalPosition, moveDuration);
        capObject.transform.parent = capOriginalParent;
        yield return MoveCap(new Vector3(-capMoveSideDistance, -capMoveUpDistance, 0f), capMoveDuration);
        SetRigidbodyConstraints(false);
        isMoving = false;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        yield return LerpPosition(transform, targetPosition, duration);
    }

    private IEnumerator RotateToAngle(Quaternion targetRotation, float duration)
    {
        yield return LerpRotation(transform, targetRotation, duration);
    }

    private IEnumerator MoveCap(Vector3 movement, float duration)
    {
        Vector3 targetPosition = capObject.transform.position + movement;
        yield return LerpPosition(capObject.transform, targetPosition, duration);
    }

    private IEnumerator StartPouringAndFilling(float pourRotationDuration)
    {
        StartCoroutine(FillCup());
        Quaternion pourRotation = Quaternion.Euler(90f, 45f, 0f);
        yield return RotateToAngle(pourRotation, pourRotationDuration);
        particleSystem.SetActive(true);
        yield return new WaitForSeconds(fillDuration);
        particleSystem.SetActive(false);
    }

    public IEnumerator FillCup()
    {
        liquidMesh.SetActive(true);
        Vector3 originalScale = liquidMesh.transform.localScale;
        Vector3 targetScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        liquidMesh.transform.localScale = new Vector3(originalScale.x, originalScale.y, /*originalScale.z*/0);
        yield return LerpScale(liquidMesh.transform, new Vector3(originalScale.x, originalScale.y, /*originalScale.z*/0), targetScale, fillDuration);
    }

    private IEnumerator LerpPosition(Transform objTransform, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = objTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            objTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        objTransform.position = targetPosition;
    }

    private IEnumerator LerpRotation(Transform objTransform, Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = objTransform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            objTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        objTransform.rotation = targetRotation;
    }

    private IEnumerator LerpScale(Transform objTransform, Vector3 startScale, Vector3 targetScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            objTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        objTransform.localScale = targetScale;
    }

    private void SetRigidbodyConstraints(bool freeze)
    {
        if (_rigidbody != null)
        {
            _rigidbody.constraints = freeze
                ? RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation
                : RigidbodyConstraints.None;
        }
    }
}

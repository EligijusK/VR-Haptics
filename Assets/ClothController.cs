using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothController : MonoBehaviour
{
    [SerializeField] public GameObject ClothUnfolding;
    [SerializeField] public GameObject ClothDropping;
    [SerializeField] public CycleThroughBlendShapes CycleThroughBlendShapesFolded;
    [SerializeField] public CycleThroughBlendShapes CycleThroughBlendShapesDropping;
    [SerializeField] public Transform destinationTransform;
    public float delayBeforeStart = 3.0f;

    private void Start()
    {
        //StartCoroutine(PlayClothSelectionSequence());
    }

    public void InteractWithCabinet()
    {
        Debug.Log("Now if I fuck this model and she just bleached her asshole...");
        StartCoroutine(PlayClothSelectionSequence());
    }

    private IEnumerator PlayClothSelectionSequence()
    {
        //yield return new WaitForSeconds(delayBeforeStart);
        yield return StartCoroutine(MoveClothUnfolding());
        yield return StartCoroutine(CycleThroughBlendShapesFolded.PlayBlendShapeAnimation());
        ChangeAnimation();
        yield return StartCoroutine(CycleThroughBlendShapesDropping.PlayBlendShapeAnimation());
    }

    public void ChangeAnimation()
    {
        ClothDropping.SetActive(true);
        ClothUnfolding.SetActive(false);
    }

    public void InterpolateToPosition()
    {
        StartCoroutine(MoveClothUnfolding());
    }

    private IEnumerator MoveClothUnfolding()
    {
        float duration = 1.0f;
        float elapsedTime = 0f;

        Vector3 startingPosition = ClothUnfolding.transform.position;
        Quaternion startingRotation = ClothUnfolding.transform.rotation;
        Vector3 startingScale = ClothUnfolding.transform.localScale;

        Vector3 targetPosition = destinationTransform.position;
        Quaternion targetRotation = destinationTransform.rotation;
        Vector3 targetScale = destinationTransform.localScale;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            ClothUnfolding.transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
            ClothUnfolding.transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, t);
            ClothUnfolding.transform.localScale = Vector3.Lerp(startingScale, targetScale, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        ClothUnfolding.transform.position = targetPosition;
        ClothUnfolding.transform.rotation = targetRotation;
        ClothUnfolding.transform.localScale = targetScale;
    }
}

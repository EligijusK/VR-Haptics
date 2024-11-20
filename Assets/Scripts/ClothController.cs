using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClothController : MonoBehaviour
{
    [SerializeField] public List<CycleThroughBlendShapes> clothItems;
    [SerializeField] public CycleThroughBlendShapes SmallClothUnfolding;
    [SerializeField] public CycleThroughBlendShapes LargeClothUnfolding;
    [SerializeField] public GameObject towelSeparate;
    [SerializeField] public XRSimpleInteractable spintelNoTowel;
    [SerializeField] public GameObject clamps;
    public float delayBeforeStart = 3.0f;
    public int currentClothIndex = 0;

    private Transform largeClothInitialTransform;
    private Transform smallClothInitialTransform;
    CycleThroughBlendShapes clothToUnfold;

    private void Start()
    {
        largeClothInitialTransform = new GameObject().transform;
        largeClothInitialTransform.position = LargeClothUnfolding.transform.position;
        largeClothInitialTransform.localScale = LargeClothUnfolding.transform.localScale;
        largeClothInitialTransform.rotation = LargeClothUnfolding.transform.rotation;

        smallClothInitialTransform = new GameObject().transform;
        smallClothInitialTransform.position = SmallClothUnfolding.transform.position;
        smallClothInitialTransform.localScale = SmallClothUnfolding.transform.localScale;
        smallClothInitialTransform.rotation = SmallClothUnfolding.transform.rotation;
        StartCoroutine(CycleThroughCloths());
    }

    private IEnumerator CycleThroughCloths()
    {
        foreach (var cloth in clothItems)
        {
            yield return StartCoroutine(PlayClothSelectionSequence(cloth));
            yield return new WaitForSeconds(delayBeforeStart);
        }
    }


    public void InteractWithCabinet()
    {
        if (clothItems.Count < currentClothIndex)
        {
            StartCoroutine(PlayClothSelectionSequence(clothItems[currentClothIndex++]));
        }
    }

    private IEnumerator PlayClothSelectionSequence(CycleThroughBlendShapes cloth)
    {
        //yield return new WaitForSeconds(delayBeforeStart);
        yield return StartCoroutine(MoveClothUnfolding(cloth));
        yield return StartCoroutine(clothToUnfold.PlayBlendShapeAnimation());
        ChangeAnimation(cloth);
        yield return StartCoroutine(cloth.PlayBlendShapeAnimation());
    }

    public void ChangeAnimation(CycleThroughBlendShapes cloth)
    {
        cloth.gameObject.SetActive(true);
        ResetUnfoldPositions();
    }

    public void ResetUnfoldPositions()
    {
        SmallClothUnfolding.gameObject.transform.position = smallClothInitialTransform.position;
        SmallClothUnfolding.gameObject.transform.localScale = smallClothInitialTransform.localScale;
        SmallClothUnfolding.gameObject.transform.rotation = smallClothInitialTransform.rotation;
        SmallClothUnfolding.ResetBlendShapes();
        SmallClothUnfolding.gameObject.SetActive(true);
        LargeClothUnfolding.gameObject.transform.position = largeClothInitialTransform.position;
        LargeClothUnfolding.gameObject.transform.localScale = largeClothInitialTransform.localScale;
        LargeClothUnfolding.gameObject.transform.rotation = largeClothInitialTransform.rotation;
        LargeClothUnfolding.ResetBlendShapes();
        LargeClothUnfolding.gameObject.SetActive(true);
    }

    private IEnumerator DisableClampsAfterTime()
    {
        yield return new WaitForSeconds(0.5f);
        clamps.SetActive(false);
    }

    private IEnumerator MoveClothUnfolding(CycleThroughBlendShapes cloth)
    {
        float duration = 1.0f;
        float elapsedTime = 0f;
        if (cloth.smallCloth)
        {
            clothToUnfold = SmallClothUnfolding;
        }
        else
        {
            clothToUnfold = LargeClothUnfolding;
            StartCoroutine(DisableClampsAfterTime());
            towelSeparate.SetActive(false);
            spintelNoTowel.enabled = false;
        }
        
        clothToUnfold.gameObject.SetActive(true);
        
        Vector3 startingPosition = clothToUnfold.transform.position;
        Quaternion startingRotation = clothToUnfold.transform.rotation;
        Vector3 startingScale = clothToUnfold.transform.localScale;

        Vector3 targetPosition = cloth.destinationTransform.position;
        Quaternion targetRotation = cloth.destinationTransform.rotation;
        Vector3 targetScale = cloth.destinationTransform.localScale;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            clothToUnfold.transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
            clothToUnfold.transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, t);
            clothToUnfold.transform.localScale = Vector3.Lerp(startingScale, targetScale, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        clothToUnfold.transform.position = targetPosition;
        clothToUnfold.transform.rotation = targetRotation;
        clothToUnfold.transform.localScale = targetScale;
    }
}

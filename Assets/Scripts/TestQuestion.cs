using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TestQuestion : MonoBehaviour
{
    [SerializeField] private TestQuestion nextQuestion;
    [SerializeField] private TMP_Text errorMessageLabel;
    [SerializeField] private string errorMessage;
    [SerializeField] private TMP_Text completionMessageLabel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject testGameobject;
    private float transitionDuration = 0.2f;
    private float flyAwayHeight = 0.2f;
    private float fadeDuration = 1.0f;

    private void Start()
    {
        completionMessageLabel.CrossFadeAlpha(0,0,false);
    }

    public void ShowErrorMessage(string message)
    {
        errorMessageLabel.text = message;
        StartCoroutine(PopErrorMessage());
    }

    public void AnsweredCorrectly()
    {
        errorMessageLabel.text = "";
        StartCoroutine(TransitionToNextQuestion());
    }

    public void AnsweredWrong()
    {
        ShowErrorMessage(errorMessage);
    }

    private IEnumerator PopErrorMessage()
    {
        Vector3 originalScale = errorMessageLabel.transform.localScale;
        Vector3 popScale = originalScale * 1.2f;

        errorMessageLabel.transform.localScale = popScale;

        float elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            errorMessageLabel.transform.localScale = Vector3.Lerp(popScale, originalScale, elapsedTime / 0.2f);
            yield return null;
        }

        errorMessageLabel.transform.localScale = originalScale;
    }

    private IEnumerator TransitionToNextQuestion()
    {
        Vector3 originalPosition = transform.position;
        Vector3 flyAwayPosition = originalPosition + new Vector3(0, flyAwayHeight, 0);  
        yield return StartCoroutine(MoveUpwardsAndAway(flyAwayPosition));
        nextQuestion.gameObject.SetActive(true);
        yield return StartCoroutine(nextQuestion.MoveIntoPosition(originalPosition));
        gameObject.SetActive(false);
    }

    private IEnumerator MoveUpwardsAndAway(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }

    public IEnumerator MoveIntoPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        transform.position = targetPosition;
    }

    public void SuccessfullyFinishTest()
    {
        StartCoroutine(CompleteTestRoutine());
    }

    private IEnumerator CompleteTestRoutine()
    {
        Vector3 originalPosition = transform.position;
        Vector3 flyAwayPosition = originalPosition + new Vector3(0, 10f, 0);  
        yield return StartCoroutine(MoveUpwardsAndAway(flyAwayPosition));
        completionMessageLabel.CrossFadeAlpha(1f,0.3f,false);
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(FadeOutCanvas());
        testGameobject.SetActive(false);
    }
    

    private IEnumerator FadeOutCanvas()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;  
    }
}

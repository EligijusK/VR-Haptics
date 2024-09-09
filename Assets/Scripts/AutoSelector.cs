using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class AutoSelector : MonoBehaviour
    {
        [SerializeField] private TestQuestion question;
        [SerializeField] private int awaitTime;
        [SerializeField] private bool finishTest;
        private void Start()
        {
            StartCoroutine(SelectButton());
        }

        private IEnumerator SelectButton()
        {
            //while (true)
            {
                if (finishTest)
                {
                    yield return new WaitForSeconds(awaitTime);
                    question.SuccessfullyFinishTest();
                    
                }
                else
                {
                    yield return new WaitForSeconds(awaitTime);
                    question.AnsweredCorrectly();
                }
            }
        }
    }
}
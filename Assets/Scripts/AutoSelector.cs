using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class AutoSelector : MonoBehaviour
    {
        [SerializeField] private TestQuestion question;
        [SerializeField] private int awaitTime;
        private void Start()
        {
            StartCoroutine(SelectButton());
        }

        private IEnumerator SelectButton()
        {
            //while (true)
            {
                yield return new WaitForSeconds(awaitTime);
                question.AnsweredCorrectly();
            }
        }
    }
}
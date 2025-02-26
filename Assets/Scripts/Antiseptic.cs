using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace DefaultNamespace
{
    public class Antiseptic : Instrument
    {
        [SerializeField] public bool correct;
        [SerializeField] public float moveDuration = 1.0f;
        [SerializeField] public float pourDuration = 0.5f;
        [SerializeField] public float holdDuration = 1.0f;
        [SerializeField] public UnityEvent correctEvent;
        public override void InteractWithItem()
        {
            if (!correct)
            {
                StartCoroutine(TextNotification._instance.ShowNotification("Pasirinktas netinkamas antiseptikas.", 3.0f));
                return;
            }
            //if (InstrumentProgressTracker._instance.bowlHasBeenPlaced)
            // {
                StartCoroutine(GetComponent<PouringAnimation>().PerformPouringAnimation(
                    targetPositionObject, 
                    moveDuration, 
                    pourDuration, 
                    holdDuration));
                SimpleInteractable.enabled = false;
                StartCoroutine(ReenableAfterCooldown());
                correctEvent.Invoke();
            // }
            //else
            //{
            //    StartCoroutine(TextNotification._instance.ShowNotification("Please select bowl first", 3.0f));
            //}
        }

        private IEnumerator ReenableAfterCooldown()
        {
            yield return new WaitForSeconds(12);
            SimpleInteractable.enabled = true;
        }

        public override void OnPlace()
        {
            //SimpleInteractable gets disabled after pour
        }
    }
}
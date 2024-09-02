using UnityEngine;

namespace DefaultNamespace
{
    public class Antiseptic : Instrument
    {
        [SerializeField] public bool correct;
        
        public override void InteractWithItem()
        {
            if (!correct)
            {
                StartCoroutine(TextNotification._instance.ShowNotification("Wrong disinfectant chosen", 3.0f));
                return;
            }
            if (InstrumentProgressTracker._instance.bowlHasBeenPlaced)
            {
                StartCoroutine(GetComponent<PouringAnimation>().PerformPouringAnimation(
                    targetPositionObject, 
                    moveDuration: 1.0f, 
                    pourRotationDuration: 0.5f, 
                    holdDuration: 1.0f));
                SimpleInteractable.enabled = false;
            }
            else
            {
                StartCoroutine(TextNotification._instance.ShowNotification("Please select bowl first", 3.0f));
            }
        }

        public override void OnPlace()
        {
            //SimpleInteractable gets disabled after pour
        }
    }
}
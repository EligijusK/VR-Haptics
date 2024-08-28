namespace DefaultNamespace
{
    public class Antiseptic : Instrument
    {

        public override void InteractWithItem()
        {
            if (InstrumentProgressTracker._instance.bowlHasBeenPlaced)
            {
                StartCoroutine(GetComponent<PouringAnimation>().PerformPouringAnimation(
                    targetPositionObject, 
                    moveDuration: 1.0f, 
                    pourRotationDuration: 0.5f, 
                    holdDuration: 1.0f));
                SimpleInteractable.enabled = false;
            }
        }

        public override void OnPlace()
        {
            //SimpleInteractable gets disabled after pour
        }
    }
}
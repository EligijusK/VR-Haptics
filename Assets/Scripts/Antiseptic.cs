namespace DefaultNamespace
{
    public class Antiseptic : Instrument
    {
        private int interactions = 0;

        public override void InteractWithItem()
        {
            
            if (interactions > 0 && onTable && InstrumentProgressTracker._instance.bowlHasBeenPlaced)
            {
                StartCoroutine(GetComponent<PouringAnimation>().PerformPouringAnimation(
                    targetPositionObject, 
                    moveDuration: 1.0f, 
                    pourRotationDuration: 0.5f, 
                    holdDuration: 1.0f));
                SimpleInteractable.enabled = false;
                interactions++;
                return;
            }
            interactions++;
            base.InteractWithItem();
        }

        public override void OnPlace()
        {
            
        }
    }
}
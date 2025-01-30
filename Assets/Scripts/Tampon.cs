namespace DefaultNamespace
{
    public class Tampon : Instrument
    {
        public override void OnPlace()
        {
            SimpleInteractable.enabled = false;
        }

        public override void InteractWithItem()
        {
            if (isMoving) return;
            //if (InstrumentProgressTracker._instance.bowlHasBeenPlaced)
            {
                StartCoroutine(MoveInstrumentToSpot(predeterminedSpot.position));
                OnPlace();
            }
            //else
           // {
           //     StartCoroutine(TextNotification._instance.ShowNotification("Pirmiausia pasirinkine dubenėlį", 3.0f));
           // }
        }
    }
}
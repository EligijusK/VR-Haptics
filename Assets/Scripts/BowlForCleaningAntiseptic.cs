namespace DefaultNamespace
{
    public class BowlForCleaningAntiseptic : Instrument
    {
        public override void OnPlace()
        {
            SimpleInteractable.enabled = false;
            InstrumentProgressTracker._instance.bowlForCleaningAntisepticHasBeenPlaced = true;
        }
    }
}
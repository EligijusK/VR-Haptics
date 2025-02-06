namespace DefaultNamespace
{
    public class Bowl : Instrument
    {
        public override void OnPlace()
        {
            InstrumentProgressTracker._instance.InstrumentPlaced();
            SimpleInteractable.enabled = false;
            InstrumentProgressTracker._instance.bowlHasBeenPlaced = true;
        }
    }
}
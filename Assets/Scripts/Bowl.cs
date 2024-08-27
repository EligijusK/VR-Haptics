namespace DefaultNamespace
{
    public class Bowl : Instrument
    {
        public override void OnPlace()
        {
            SimpleInteractable.enabled = false;
            InstrumentProgressTracker._instance.bowlHasBeenPlaced = true;
        }
    }
}
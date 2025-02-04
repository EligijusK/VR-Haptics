using UnityEngine;

namespace DefaultNamespace
{
    public class InstrumentProgressTracker : MonoBehaviour
    {
        public static InstrumentProgressTracker _instance;
        public bool bowlHasBeenPlaced;
        public bool bowlForCleaningAntisepticHasBeenPlaced;
        private int instrumentsOnTableCount = 0;
        private void Start()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        public void InstrumentPlaced()
        {
            instrumentsOnTableCount++;
            Debug.Log("Instrument placed. Count = " + instrumentsOnTableCount);
            
            if (instrumentsOnTableCount == 3)
            {
                AudioManager.Instance.TakeTampon();
            }
        }
    }
}
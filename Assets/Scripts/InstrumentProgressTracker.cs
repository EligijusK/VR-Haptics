using UnityEngine;

namespace DefaultNamespace
{
    public class InstrumentProgressTracker : MonoBehaviour
    {
        public static InstrumentProgressTracker _instance;
        public bool bowlHasBeenPlaced;
        public bool bowlForCleaningAntisepticHasBeenPlaced;
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
    }
}
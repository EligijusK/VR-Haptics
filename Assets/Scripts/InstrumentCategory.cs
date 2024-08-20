using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class InstrumentCategory
    {
        [SerializeField] public Categories category = Categories.Default;
        [SerializeField] public int requiredCount = 0;
        [SerializeField] public int currentCount = 0;
    }

    public enum Categories
    {
        Default,
        Scissors,
        Antiseptic
    }
}

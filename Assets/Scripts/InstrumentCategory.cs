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
        [SerializeField] public bool optional = false;
    }

    public enum Categories
    {
        Spaustukas,
        SpaustukasBlogas,
        InkstoDubenelis,
        Tamponas,
        UzklotuSegtukas,
        Skalpelis,
        Pincetas,
        Pincetas12,
        SiuluZirkles,
        Adatkotis,
        ZaizdosKrastuSkiediklis,
        Default,
        Scissors,
        Antiseptic,
        Bowl,
        Secret,
        SkalpelioAntgalis,
        Vice,
        Svirkstas,
        Pleistras
    }
}

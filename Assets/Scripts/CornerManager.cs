using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerManager : MonoBehaviour
{
    [SerializeField] private ClothController _clothController;
    public int numberOfAddedClamps = 0;


    public void AddedClamp()
    {
        numberOfAddedClamps++;
        if (numberOfAddedClamps == 4)
        {
            _clothController.SetClampsInPlaceFlag(true);
            AudioManager.Instance.ChooseLastCloth();
        }
    }
}

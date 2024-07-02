using System.Collections.Generic;
using UnityEngine;
using WeArt.Components;
using WeArt.Core;

namespace WeArt.Utils.Actuation
{
    /// <summary>
    /// WeArtHapticActuationPanel — contains the Actuation data of haptic elements of set hands. Useful for debug.  
    /// </summary>
    public class WeArtHapticActuationPanel : MonoBehaviour
    {
        [SerializeField] internal WeArtHandController leftHandObject;
        [SerializeField] internal WeArtHandController rightHandObject;

        [SerializeField] internal List<WeArtHapticActuationElement> leftActuationElements;
        [SerializeField] internal List<WeArtHapticActuationElement> rightActuationElements;

        private void Awake()
        {
            if (leftHandObject) SetHandHapticElementsForTacking(leftActuationElements, leftHandObject);
            if (rightHandObject) SetHandHapticElementsForTacking(rightActuationElements, rightHandObject);
        }
        
        /// <summary>
        /// Shows/Hides this panel
        /// </summary>
        public void ShowHidePanel()
        {
            gameObject.SetActive(!isActiveAndEnabled);
        }
        
        /// <summary>
        /// Links the haptic elements from hands to actuation fields at the panel. 
        /// </summary>
        /// <param name="hapticPanelElements"></param>
        /// <param name="hand"></param>
        private void SetHandHapticElementsForTacking(List<WeArtHapticActuationElement> hapticPanelElements,
            WeArtHandController hand)
        {
            hapticPanelElements[0].SetHapticObject(hand._thumbThimbleHaptic);
            hapticPanelElements[1].SetHapticObject(hand._indexThimbleHaptic);
            hapticPanelElements[2].SetHapticObject(hand._middleThimbleHaptic);
        }
    }
}

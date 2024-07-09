using UnityEditor;
using UnityEngine;

public class SelectedItem : MonoBehaviour
{ 
        [SerializeField] bool correctItemIsSelected = false;
        [SerializeField] private Item selectedItem;
        public void ItemIsSelected(Item item)
        {
                if (selectedItem != null)
                {
                        selectedItem.DeselectItem();
                }
                selectedItem = item;
                correctItemIsSelected = item.IsItemCorrect();
        }
        
        public void DeselectItem()
        {
                if (selectedItem != null)
                { 
                        selectedItem.DeselectItem();
                }
                selectedItem = null;
                correctItemIsSelected = false;
        }
}

using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [SerializeField] SelectedItem selectedItemManager;
    [SerializeField] bool isCorrectItem;
    public UnityEvent OnItemSelect;
    public UnityEvent OnItemDeselect;
    public void ItemIsSelected()
    {
        selectedItemManager.ItemIsSelected(this);
        OnItemSelect.Invoke();
    }
    
    public bool IsItemCorrect()
    {
        return isCorrectItem;
    }

    public void DeselectItem()
    {
        OnItemDeselect.Invoke();
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class OrderingInLine : MonoBehaviour
{
    [SerializeField] OpenDoors _openDoors;
    public bool orderFromLeftToRight = true;
    
    public OrderElement firstElement;
    public OrderElement lastElement;
    
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangedOrderInList()
    {
        int index = 0;
        bool orderIsCorrect = true;
        for (OrderElement element = firstElement; element.nextElement != null; element = element.nextElement)
        {
            if (element.order != index)
            {
                orderIsCorrect = false;
                break;
            }
            index++;
        }
        Debug.Log(orderIsCorrect);
        if (orderIsCorrect)
        {
            _openDoors.AddForce();
        }
    }

    public void StartBlinking()
    {
        for (OrderElement element = firstElement; element.nextElement != null; element = element.nextElement)
        {
            element.GetOutline().StartBlinking();
            
        }
    }
    
    public void StartBlinkingTimer()
    {
        for (OrderElement element = firstElement; element.nextElement != null; element = element.nextElement)
        {
            element.GetOutline().StartBlinkingTimer();
            
        }
    }
    
    public void StopBlinking()
    {
        for (OrderElement element = firstElement; element.nextElement != null; element = element.nextElement)
        {
            element.GetOutline().StopBlinking();
        }
    }

}

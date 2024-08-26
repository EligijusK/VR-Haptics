using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

public class OrderingInLine : MonoBehaviour
{
    [SerializeField] OpenDoors _openDoors;
    [SerializeField] private HandWashingTest handWashingTest;
    
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

    public void HandWashTest(int index)
    {
        handWashingTest.CheckIfElementIsInCorrectPlace(index);
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

    public bool CheckOrderInList(int index)
    {
        int tempIndex = 0;
        bool orderIsCorrect = true;
        for (OrderElement element = firstElement; element.nextElement != null; element = element.nextElement)
        {
            if (tempIndex == index && element.order != tempIndex)
            {
                return false;
            }
            tempIndex++;
        }
        return true;
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

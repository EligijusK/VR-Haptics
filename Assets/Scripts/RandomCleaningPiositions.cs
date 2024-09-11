using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCleaningPiositions : MonoBehaviour
{
    [SerializeField]
    OrderingInLine orderingInLine;
    [SerializeField]
    List<Transform> positions;
    [SerializeField]
    List<OrderElement> orderInLineElements;

    private List<OrderElement> newOrderList;
    private int index = 0;

    private void Awake()
    {
        GenerateNewPositions();
    }

    private void GenerateNewPositions()
    {
        List<OrderElement> tempOrderList = new List<OrderElement>();
        foreach (OrderElement element in orderInLineElements)
        {
            tempOrderList.Add(element);
        }
        newOrderList = new List<OrderElement>();
        index = 0;
        while (index < orderInLineElements.Count)
        {
            
            int randomIndex = UnityEngine.Random.Range(0, tempOrderList.Count-1);
            OrderElement element = tempOrderList[randomIndex];
            tempOrderList.RemoveAt(randomIndex);
            if (newOrderList.Count > 0)
            {
                newOrderList[index - 1].nextElement = element;
                element.previousElement = newOrderList[index - 1];
            }
            else
            {
                element.previousElement = null;
            }
            element.transform.position = positions[index].position;
            newOrderList.Add(element);
            index++;
        }
        orderingInLine.firstElement = newOrderList[0];
       
    }
}

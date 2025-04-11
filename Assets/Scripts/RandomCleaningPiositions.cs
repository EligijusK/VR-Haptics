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

    public List<OrderElement> newOrderList;
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
    public void CompareLists()
    {
        // If sizes differ, automatically incorrect
        if (newOrderList.Count != orderInLineElements.Count)
        {
            Debug.Log("Incorrect");
            return;
        }

        // Compare element by element
        for (int i = 0; i < orderInLineElements.Count; i++)
        {
            if (newOrderList[i] != orderInLineElements[i])
            {
                Debug.Log("Incorrect");
                return;
            }
        }

        // If we get here, all elements match in order
        Debug.Log("Correct");
    }
}

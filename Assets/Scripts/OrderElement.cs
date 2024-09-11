using System;
using System.Threading;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

public class OrderElement : MonoBehaviour
{
       public OrderingInLine reference;
       public int order;
       private Vector3 startingPosition;
       public OrderElement previousElement;
       public OrderElement nextElement;
       Outline outline;
       private Vector3 previousPosition;
       private bool startTracking = false;
       private Vector3 currentPosition;
       void Start()
       {
              startTracking = false;
              startingPosition = transform.position;
              outline = GetComponent<Outline>();
       }
       
       void Update()
       {
              if (!transform.position.Equals(previousPosition))
              {
                     ComparePosition();
                     previousPosition = transform.position;
              }
       }
       
       private void ComparePosition()
       {
              if(previousElement != null && transform.position.x < previousElement.transform.position.x)
              {
                      // previous
                      if (previousElement.previousElement != null)
                      {
                             OrderElement temp = previousElement.previousElement;
                             OrderElement tempPrev = previousElement;

                             temp.nextElement = this;
                             this.previousElement = temp;

                             tempPrev.nextElement = this.nextElement;
                             if (this.nextElement != null)
                             {
                                    this.nextElement.previousElement = tempPrev;
                             }

                             this.nextElement = tempPrev;

                             tempPrev.previousElement = this;
                             
                             Vector3 tempPos = this.startingPosition;
                             tempPrev.transform.position = tempPos;
                             this.startingPosition = tempPrev.startingPosition;
                             tempPrev.startingPosition = tempPos;
                             reference.ChangedOrderInList();
                      }
                      else
                      {
                             OrderElement tempPrev = previousElement;
                             this.previousElement = null;
                             tempPrev.nextElement = this.nextElement;
                             if (this.nextElement != null)
                             {
                                    this.nextElement.previousElement = tempPrev;
                             }
                             this.nextElement = tempPrev;
                             tempPrev.previousElement = this;
                             
                             Vector3 tempPos = this.startingPosition;
                             tempPrev.transform.position = tempPos;
                             this.startingPosition = tempPrev.startingPosition;
                             tempPrev.startingPosition = tempPos;
                             
                             reference.firstElement = this;
                             reference.ChangedOrderInList();
                      }

                      Debug.Log("Previous element");
              }
              else if (nextElement != null && transform.position.x > nextElement.transform.position.x)
              {
                     if (nextElement.nextElement != null)
                     {
                            OrderElement temp = nextElement.nextElement;
                            OrderElement tempNext = nextElement;

                            temp.previousElement = this;
                            this.nextElement = temp;

                            tempNext.previousElement = this.previousElement;
                            if (this.previousElement != null)
                            {
                                   this.previousElement.nextElement = tempNext;
                            }
                            this.previousElement = tempNext;

                            tempNext.nextElement = this;
                            
                            Vector3 tempPos = this.startingPosition;
                            tempNext.transform.position = tempPos;
                            this.startingPosition = tempNext.startingPosition;
                            tempNext.startingPosition = tempPos;
                            
                            reference.ChangedOrderInList();
                     }
                     else
                     {
                            OrderElement tempNext = nextElement;
                            this.nextElement = null;
                            tempNext.previousElement = this.previousElement;
                            if (this.previousElement != null)
                            {
                                   this.previousElement.nextElement = tempNext;
                            }
                            this.previousElement = tempNext;
                            tempNext.nextElement = this;
                            
                            Vector3 tempPos = this.startingPosition;
                            tempNext.transform.position = tempPos;
                            this.startingPosition = tempNext.startingPosition;
                            tempNext.startingPosition = tempPos;
                            
                            reference.ChangedOrderInList();
                     }
                     Debug.Log("Next element");
              }

              if (nextElement == null && transform.position.x > previousElement.transform.position.x)
              {
                     // end
                     Debug.Log("end element");
              }
              else if (previousElement == null && transform.position.x < nextElement.transform.position.x)
              {
                     // start
                     Debug.Log("start element");
              }
       }
       
       public void StartTracking()
       {
              startTracking = true;
       }
       
       public void CheckIfCorrect()
       {
              if (startTracking)
              {
                     reference.HandWashTest(order);
              }
              startTracking = false;
       }
       
       public Outline GetOutline()
       {
              return outline;
       }
     
}
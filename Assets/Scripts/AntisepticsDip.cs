using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntisepticsDip : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.tag == "Tampon" && !other.transform.parent.GetComponent<TamponInAntyseptics>().CanTamponBeUsed())
        {
            other.transform.parent.GetComponent<TamponInAntyseptics>().DipInAntiseptics();
            Debug.Log("Tampon was in antyseptics");
        }
    }
}

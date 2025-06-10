using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManager : MonoBehaviour
{
    private GameObject[] prefabsToPersist;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // void Start()
    // {
    //     foreach (var prefab in prefabsToPersist)
    //     {
    //         GameObject go = Instantiate(prefab);
    //         go.transform.SetParent(transform, worldPositionStays: true);
    //     }
    // }
}

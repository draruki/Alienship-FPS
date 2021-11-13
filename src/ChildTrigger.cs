using UnityEngine;
using System.Collections;

public class ChildTrigger : MonoBehaviour 
{
    public ParentTrigger parentTrigger;

    void Awake()
    {
        parentTrigger = GetComponentInParent<ParentTrigger>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        parentTrigger.childOnTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        parentTrigger.childOnTriggerStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        parentTrigger.childOnTriggerExit(other);
    }
}

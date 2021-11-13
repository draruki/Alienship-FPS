using System;
using UnityEngine;

public abstract class ParentTrigger : MonoBehaviour
{
    public abstract void childOnTriggerEnter(Collider other);
    public abstract void childOnTriggerStay(Collider other);
    public abstract void childOnTriggerExit(Collider other);
}
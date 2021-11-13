using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour 
{
    public float speed = 4.8f;
    
    void Awake () 
    {
        
    }

    public void open()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class Decal : MonoBehaviour 
{
    [HideInInspector]
    public float lifetime;

    public void setDirectionNormal(Vector3 normal)
    {
        transform.LookAt(transform.position + normal);
        transform.Translate(normal * 0.02f);
        transform.Rotate(normal, Random.Range(0, 359), Space.World);
    }
	
	void Update ()
    {
        lifetime -= Time.deltaTime;
	}
}

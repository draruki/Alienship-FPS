using UnityEngine;
using System.Collections;

public class RotateY : MonoBehaviour 
{
	public float speed = 1f;

	void Start () 
    {
	
	}
	
	void Update () 
    {
		gameObject.transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
	}
}

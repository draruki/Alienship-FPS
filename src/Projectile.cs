using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour 
{
    [HideInInspector]
    public float lifetime = 100f;
    private float speed = 30f;
    [HideInInspector]
    public Vector3 direction;

    void Start ()
    {

    }

	void Awake () 
    {
        direction = Vector3.zero;
	}
	
	void Update () 
    {
        if (GC.PAUSED)
            return;

        lifetime -= Time.deltaTime;
        transform.position += direction * speed * Time.deltaTime;
	}

    public void setDirection(Vector3 direction)
    {
        this.direction = direction;
        transform.rotation = Quaternion.FromToRotation(transform.forward, direction);
    }
}
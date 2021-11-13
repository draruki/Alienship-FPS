using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SphereCollider))]
public class Debaser : MonoBehaviour 
{
    public enum ACTION_TYPE {ENABLE, DISABLE};
    public ACTION_TYPE action;
    public GameObject[] objects;
    public bool initialDisable;

	void Start () 
    {
        if (initialDisable)
        {
            foreach (GameObject g in objects)
            {
                g.SetActive(false);
            }
        }
	}

    public void activate()
    {
        if (action == ACTION_TYPE.ENABLE)
        {
            foreach (GameObject g in objects)
            {
                g.SetActive(true);
            }
        }
        else if (action == ACTION_TYPE.DISABLE)
        {
            foreach (GameObject g in objects)
            {
                g.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerEntity player = other.GetComponent<PlayerEntity>();
        if (player == null)
            return;

        activate();
    }
}

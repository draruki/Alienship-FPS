using UnityEngine;
using System.Collections;

public abstract class Pickup : MonoBehaviour 
{
    public GameObject model;
    public bool playerTaken;
    private float floatSpeed;
	private float rotateSpeed;
    [HideInInspector]
    private AudioSource audioSource;

    public virtual void Start () 
	{
        floatSpeed = 50f;
        rotateSpeed = 120f;
        audioSource = GetComponent<AudioSource>();
	}
	
	public virtual void Update () 
	{
        if (GC.PAUSED)
            return;
        
        model.transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
    }

    protected virtual void givePickup(PlayerEntity player)
    {
        audioSource.clip = Assets.instance.pickupSound;
        audioSource.volume = Assets.instance.pickupSoundVolume * GC.SOUND_VOLUME;
        audioSource.Play();
        kill();
        playerTaken = true;
    }

    public void kill()
    {
        GetComponent<SphereCollider>().enabled = false;
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].enabled = false;
        }
        this.enabled = false;
    }

    public void respawn()
    {
        GetComponent<SphereCollider>().enabled = true;
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].enabled = true;
        }
        this.enabled = true;
        playerTaken = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerEntity player = other.gameObject.GetComponent<PlayerEntity>();
        if (player != null)
        {
            givePickup(player);
        }
    }
}
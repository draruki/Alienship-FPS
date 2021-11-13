using UnityEngine;
using System.Collections;

public class BaseEntity : MonoBehaviour 
{
    protected CharacterController characterController;
    public float moveSpeed = 3f;
    public int health = 30;
    protected int maxHealth;
    private const float FALL_Y = -20f;
    protected AudioSource audioSource;

    public virtual void Awake ()
    {
        maxHealth = health;

        characterController = gameObject.GetComponent<CharacterController>();

        audioSource = GetComponent<AudioSource>();
    }

	public virtual void Start ()
    {
	}
	
	public virtual void Update ()
    {
        if (transform.position.y < FALL_Y)
        {
            damaged(health, this);
        }
	}

    protected void setHealth(int h)
    {
        health = h;
        maxHealth = h;
    }

    public void resetHealth()
    {
        health = maxHealth;
    }

    public void addHealth(int v)
    {
        health += v;

        if (health > maxHealth)
            health = maxHealth;
    }

    public virtual bool Move(Vector3 direction)
    {
        direction.y = 0;
        
        bool isGrounded = characterController.SimpleMove(direction * moveSpeed);
        return isGrounded;
    }

    public virtual void damaged(int damage, BaseEntity attacker)
    {
        health -= damage;
    }
}
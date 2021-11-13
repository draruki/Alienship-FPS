using UnityEngine;
using System.Collections;

public class EnemyEntity : BaseEntity
{
    // VFX
    private MeshRenderer[] meshRenderers;
    private float flashTimer;
    private Material[] defaultMaterials;
    private const float DEATH_FADE = 0.03f;
    private const float FLASH_TIME = 0.155f;

    // AI
    private AIController aicontroller;

    // spawn
    private Quaternion spawnRotation;
    private Vector3 spawnPosition;

    // combat
    private float attackTimer;
    private BaseEntity target;
    public float attackSpeed = 0.9f;
    public float attackDistance;
    public bool dead;
    public bool ranged;
    public int damage = 2;
    private bool dying;

    // audio
    public AudioClip deathSound;
    [Range(0, 1f)] public float deathSoundVolume;
    public AudioClip hitSound;
    [Range(0, 1f)] public float hitSoundVolume;
    public AudioClip attackSound;
    [Range(0, 1f)] public float attackSoundVolume;

    public override void Awake()
    {
        base.Awake();

        // VFX
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        defaultMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            defaultMaterials[i] = meshRenderers[i].sharedMaterial;
        }
        dying = false;
        dead = false;

        // AI
        aicontroller = GetComponent<AIController>();

        // spawn
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

	public override void Start () 
    {
        base.Start();

        attackTimer = attackSpeed;
	}
	
	public override void Update () 
    {
        if (GC.PAUSED)
            return;
        
        base.Update();

        if (dying)
        {
            // alpha
            Color col;
            for (int i = 0; i < meshRenderers.Length; ++i)
            {
                col = meshRenderers[i].material.color;
                col.a -= DEATH_FADE;
                meshRenderers[i].material.color = col;

                if (col.a <= 0)
                {
                    kill();
                }
            }
        }
        // flash
        else if (flashTimer != 0)
        {
            flashTimer -= Time.deltaTime;


            if (flashTimer < 0)
                stopFlash();
        }

        // attack
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
	}

    public void respawn(Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].enabled = true;
        }

        aicontroller.enabled = true;
        characterController.enabled = true;
        this.enabled = true;
        target = null;
        health = maxHealth;
        dying = false;
        dead = false;
        transform.position = position;
        transform.rotation = rotation;
    }
    public void respawn(Vector3 position)
    {
        respawn(position, spawnRotation);
    }
    public void respawn()
    {
        respawn(spawnPosition, spawnRotation);
    }
    public void kill()
    {
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material = defaultMaterials[i];
            meshRenderers[i].enabled = false;
        }

        aicontroller.kill();
        aicontroller.enabled = false;
        characterController.enabled = false;
        this.enabled = false;
        dead = true;
        dying = false;
    }
    private void cosmeticKill()
    {
        if (dying)
            return;
        
        dying = true;
        aicontroller.enabled = false;
        characterController.enabled = false;
        
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material = Assets.instance.deathMaterial;
            meshRenderers[i].material.color = new Color(1, 0, 0, 0.4f);
        }
    }

    public override bool Move(Vector3 direction)
    {
        direction.y = 0;
        bool isGrounded = characterController.SimpleMove(direction * moveSpeed);
        transform.LookAt(transform.position + direction, Vector3.up);

        return isGrounded;
    }

    public void attack(BaseEntity target)
    {
        if (dying)
            return;
        
        if (attackTimer <= 0)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Vector3 lookDirection = direction;
            lookDirection.y = 0;
            lookDirection.Normalize();
            transform.LookAt(transform.position + lookDirection);

            if (ranged)
            {
                RaycastHit hitInfo;
                if (Physics.Linecast(transform.position, target.transform.position, out hitInfo))
                {
                    BaseEntity hitEntity = hitInfo.collider.GetComponent<BaseEntity>();
                    if (hitEntity != null)
                    {
                        this.target = target;
                        ParticleManager.shootPistolBullet(transform.position + direction * 1.1f, direction);
                        float distance = Vector3.Distance(transform.position, target.transform.position);
                        float shootDelay = distance * 0.025f;
                        attackTimer = attackSpeed + shootDelay;
                        Invoke("shoot", shootDelay);
                    }
                }
            }
            else
            {
                target.damaged(damage, this);
                attackTimer = attackSpeed;
            }
        }
    }
    private void shoot()
    {
        if (target == null)
            return;

        RaycastHit hitInfo;
        if (Physics.Linecast(transform.position, target.transform.position, out hitInfo))
        {
            BaseEntity hitEntity = hitInfo.collider.GetComponent<BaseEntity>();
            if (hitEntity != null)
            {
                target.damaged(damage, this);
            }
        }
    }

    public override void damaged(int damage, BaseEntity attacker)
    {
        if (dying)
            return;
        
        base.damaged(damage, attacker);

        audioSource.clip = hitSound;
        audioSource.volume = hitSoundVolume * GC.SOUND_VOLUME;
        audioSource.Play();

        if (health <= 0)
        {
            bool fainaruFantaji = false;

            if (fainaruFantaji)
            {
                audioSource.clip = Assets.instance.ff7DeathSound;
                audioSource.volume = Assets.instance.ff7DeathSoundVolume * GC.SOUND_VOLUME;
            }
            else
            {
                audioSource.clip = deathSound;
                audioSource.volume = deathSoundVolume * GC.SOUND_VOLUME;
            }

            audioSource.Play();
            cosmeticKill();
        }
        else
        {
            flash();
        }

        aicontroller.damaged(attacker);

    }

    private void flash()
    {
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material = Assets.instance.flashMaterial;
            meshRenderers[i].material.color = new Color(1, 1, 1, 1f);
        }

        flashTimer = FLASH_TIME;
    }
    private void stopFlash()
    {
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material = defaultMaterials[i];
        }

        flashTimer = 0f;
    }
}
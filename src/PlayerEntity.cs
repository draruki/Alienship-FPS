using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerEntity : BaseEntity
{
    // main
    public Camera gameCamera;
    public GameManager gameManager;

    // audio
    public AudioClip shootSound;

    // weapons
    [HideInInspector]
    public Weapon currentWeapon = WeaponManager.pistol;
    private List<GameObject> weaponModels;
    public int spreadAmmo;
    public int singleAmmo;
    public int autoAmmo;
    public Transform weaponModelTransform;

    // shoot
    private Ray shootRay;
    private Quaternion lastCameraQuaternion;
    public int shotgunRays = 30;
    private float weaponCooldown;
    public float mouseSensitivity = 3.5f;
    public Transform muzzleTransform;
    public int raysPerDecal = 1;

    // trail
    public MeshRenderer trail;

    // muzzle flash
    private float muzzleFlashTimer;
    public float muzzleFlashTime = 0.55f;
    public MeshRenderer muzzleFirePlane;

    // crouch
    private float standingRadius;
    private float standingHeight;
    private float crouchRadius = 0.35f;
    private bool forceUncrouch;
    private float crouchHeight = 0.73f;
    private bool canUncrouch;
    private bool crouched;
    private float crouchCameraDeltaHeight = 0.42f;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        setHealth(100);
        shootRay = new Ray();
        lastCameraQuaternion = new Quaternion();

        // crouch
        standingRadius = characterController.radius;
        standingHeight = characterController.height;
        crouched = false;
        forceUncrouch = false;
        canUncrouch = true;

        // weapons
        setWeapon(WeaponManager.pistol);
        weaponModels = new List<GameObject>();
        trail.enabled = false;
    }
	
    public override void Update()
    {
        if (GC.PAUSED)
            return;
        
        base.Update();

        /*
        Vector3 up = new Vector3(Mathf.Abs(gameCamera.transform.up.x),Mathf.Abs(gameCamera.transform.up.y),Mathf.Abs(gameCamera.transform.up.z));
        Vector3 right = new Vector3(Mathf.Abs(gameCamera.transform.right.x),Mathf.Abs(gameCamera.transform.right.y),Mathf.Abs(gameCamera.transform.right.z));
        print("camera up:  " + up + "   camera right:  " + right);
        */

        weaponCooldown -= Time.deltaTime;

        if (forceUncrouch)
            disableCrouch();

        float distance = 1.1f;
        Vector3 origin = new Vector3(transform.position.x + characterController.center.x, transform.position.y + characterController.center.y + crouchHeight/2, transform.position.z + characterController.center.z);
        canUncrouch = !Physics.Raycast(new Ray(origin, Vector3.up), distance);

        if (muzzleFlashTimer > 0)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0)
                disableMuzzleFlash();
        }

        /*
        print(canUncrouch);
        if (rh.collider == null)
            print("null");
        else
            print(rh.collider.name);
        */
    }

    private void enableMuzzleFLash()
    {
        muzzleFirePlane.transform.Rotate(Vector3.forward, Random.Range(0, 360), Space.Self);
        muzzleFlashTimer = muzzleFlashTime;
        muzzleFirePlane.enabled = true;
        trail.enabled = true;
    }
    private void disableMuzzleFlash()
    {
        trail.enabled = false;
        muzzleFirePlane.enabled = false;
    }

    public void mouseLook(float deltaX, float deltaY)
    {
        Quaternion xQuaternion = Quaternion.AngleAxis (deltaX * mouseSensitivity, Vector3.up);
        transform.rotation = transform.rotation * xQuaternion;

        lastCameraQuaternion = gameCamera.transform.rotation;
        Quaternion yQuaternion = Quaternion.AngleAxis (deltaY * mouseSensitivity, -Vector3.right);
        gameCamera.transform.rotation = gameCamera.transform.rotation * yQuaternion;

        if (gameCamera.transform.localRotation.x <= -0.7f || gameCamera.transform.localRotation.x >= 0.7f)
        {
            gameCamera.transform.rotation = lastCameraQuaternion;
        }
    }

    public void enableCrouch()
    {
        if (crouched == false)
        {
            characterController.radius = crouchRadius;
            characterController.height = crouchHeight;
            transform.Translate(0, -((standingHeight-crouchHeight)/2), 0);
            gameCamera.transform.Translate(0, -crouchCameraDeltaHeight, 0);
            crouched = true;
            moveSpeed *= 0.5f;
        }
    }
    public void disableCrouch()
    {
        if (crouched == true)
        {
            if (canUncrouch && characterController.isGrounded)
            {
                characterController.radius = standingRadius;
                characterController.height = standingHeight;
                transform.Translate(0, (standingHeight-crouchHeight)/2, 0);
                gameCamera.transform.Translate(0, crouchCameraDeltaHeight, 0);
                crouched = false;
                forceUncrouch = false;
                moveSpeed *= 2;
            }
            else
            {
                forceUncrouch = true;
            }
        }
    }

    public void setWeapon(Weapon weapon)
    {
        if (weapon == WeaponManager.shotgun && !gameManager.playerHasShotgun)
            return;
        if (weapon == WeaponManager.rifle && !gameManager.playerHasRifle)
            return;

        currentWeapon = weapon;

        if (weaponModels.Count != 0)
        {
            Destroy(weaponModels[0]);
            weaponModels.Clear();
        }
        
        weaponModels.Add(Instantiate(currentWeapon.model));
        weaponModels[0].transform.parent = gameCamera.transform;
        weaponModels[0].transform.position = weaponModelTransform.position;
        weaponModels[0].transform.rotation = weaponModelTransform.rotation;
        weaponModels[0].transform.localScale = weaponModelTransform.localScale;

        Transform[] ts = weaponModels[0].GetComponentsInChildren<Transform>();
        foreach(Transform t in ts)
        {
            t.gameObject.layer = 8;
        }

        muzzleFirePlane.enabled = false;
        muzzleFirePlane.transform.localPosition = currentWeapon.muzzleFlashPosition;
        muzzleFirePlane.sharedMaterial = currentWeapon.muzzleFlashMaterial;

        gameManager.onChangedWeapon(weapon);
    }

    public void nextWeapon()
    {
        if (currentWeapon == WeaponManager.shotgun && !gameManager.playerHasRifle)
            setWeapon(WeaponManager.pistol);
        else if (currentWeapon == WeaponManager.pistol)
            setWeapon(WeaponManager.shotgun);
        else if (currentWeapon == WeaponManager.rifle)
            setWeapon(WeaponManager.pistol);
        else if (currentWeapon == WeaponManager.shotgun)
            setWeapon(WeaponManager.rifle);
    }
    public void previousWeapon()
    {
        if (currentWeapon == WeaponManager.pistol && !gameManager.playerHasRifle)
            setWeapon(WeaponManager.shotgun);
        else if (currentWeapon == WeaponManager.pistol)
            setWeapon(WeaponManager.rifle);
        else if (currentWeapon == WeaponManager.rifle)
            setWeapon(WeaponManager.shotgun);
        else if (currentWeapon == WeaponManager.shotgun)
            setWeapon(WeaponManager.pistol);
    }

    public void addWeapon(WeaponManager.WEAPON type)
    {
        gameManager.onNewWeapon(type);
    }

    public int getAmmo()
    {
        if (currentWeapon.type == Weapon.TYPE.SINGLE)
            return singleAmmo;
        if (currentWeapon.type == Weapon.TYPE.AUTO)
            return autoAmmo;
        if (currentWeapon.type == Weapon.TYPE.SPREAD)
            return spreadAmmo;
        return -1;
    }

    public void addAmmo(int a, Weapon.TYPE type)
    {
        if (type == Weapon.TYPE.SINGLE)
            singleAmmo +=a;
        else if (type == Weapon.TYPE.AUTO)
            autoAmmo +=a;
        else if (type == Weapon.TYPE.SPREAD)
            spreadAmmo +=a;
    }

    public void shoot()
    {
        if (weaponCooldown <= 0)
        {
            if (currentWeapon.type == Weapon.TYPE.SINGLE)
            {
                if (singleAmmo > 0 || GC.INFINITE_AMMO)
                {
                    shootSingle();
                    singleAmmo--;
                }
            }
            else if (currentWeapon.type == Weapon.TYPE.AUTO)
            {
                if (autoAmmo > 0 || GC.INFINITE_AMMO)
                {
                    shootSingle();
                    autoAmmo--;
                }
            }
            else if (currentWeapon.type == Weapon.TYPE.SPREAD)
            {
                shootSpread();
            }
        }
    }

    private void shootSpread()
    {
        if (spreadAmmo > 0 || GC.INFINITE_AMMO)
        {
            shootRay.direction = gameCamera.transform.forward;
            shootRay.origin = gameCamera.transform.position;
            Invoke("shootSpreadRaycast", currentWeapon.delay);
            weaponCooldown = currentWeapon.cooldown;

            if (currentWeapon.shootSound != null)
            {
                audioSource.clip = currentWeapon.shootSound;
                audioSource.volume = currentWeapon.shootVolume * GC.SOUND_VOLUME;
                audioSource.Play();
            }
        }
    }
    private void shootSpreadRaycast()
    {
        RaycastHit hitInfo = new RaycastHit();

        for (int i = 0; i < shotgunRays; ++i)
        {
            Vector3 shootDirection = randomSpreadDirection(shootRay.direction);
            Physics.Raycast(new Ray(shootRay.origin, shootDirection), out hitInfo, currentWeapon.distance);

            if (hitInfo.collider != null)
            {
                BaseEntity entity = hitInfo.collider.gameObject.GetComponent<BaseEntity>();
                if (entity != null && entity != this)
                {
                    float hitDistance = hitInfo.distance;
                    entity.damaged(currentWeapon.damage, this);
                }
                else if (i % raysPerDecal == 0 && hitInfo.collider.tag == "decalSurface")
                {
                    ParticleManager.plasterDecal(hitInfo.point, hitInfo.normal);
                }
            }
        }

        enableMuzzleFLash();
        trail.sharedMaterial = Assets.instance.shotgunTrailMaterial;
        spreadAmmo--;
    }
    private Vector3 randomSpreadDirection(Vector3 dir)
    {
        Vector3 spreadDir = new Vector3(dir.x, dir.y, dir.z);
        Vector2 randomCirclePoint = Random.insideUnitCircle;

        spreadDir.x += randomCirclePoint.x * currentWeapon.spread * gameCamera.transform.right.x;
        spreadDir.x += randomCirclePoint.y * currentWeapon.spread * gameCamera.transform.up.x;
        spreadDir.y += randomCirclePoint.x * currentWeapon.spread * gameCamera.transform.right.y;
        spreadDir.y += randomCirclePoint.y * currentWeapon.spread * gameCamera.transform.up.y;
        spreadDir.z += randomCirclePoint.x * currentWeapon.spread * gameCamera.transform.right.z;
        spreadDir.z += randomCirclePoint.y * currentWeapon.spread * gameCamera.transform.up.z;

        return spreadDir;
    }

    private void shootSingle()
    {
        shootRay.direction = gameCamera.transform.forward;
        shootRay.origin = gameCamera.transform.position;
        Invoke("shootSingleRaycast", currentWeapon.delay);
        weaponCooldown = currentWeapon.cooldown;

        if (currentWeapon.shootSound != null)
        {
            audioSource.clip = currentWeapon.shootSound;
            audioSource.volume = currentWeapon.shootVolume * GC.SOUND_VOLUME;
            audioSource.Play();
        }
    }
    private void shootSingleRaycast()
    {
        Physics.Raycast(new Ray(shootRay.origin, shootRay.direction), out hitInfo, currentWeapon.distance);
        RaycastHit hitInfo = new RaycastHit();

        if (hitInfo.collider != null)
        {
            BaseEntity entity = hitInfo.collider.gameObject.GetComponent<BaseEntity>();
            if (entity != null && entity != this)
            {
                entity.damaged(currentWeapon.damage, this);
            }
            else if (hitInfo.collider.tag == "decalSurface")
            {
                ParticleManager.plasterDecal(hitInfo.point, hitInfo.normal);
            }
        }

        enableMuzzleFLash();

        if (currentWeapon == WeaponManager.pistol)
            trail.sharedMaterial = Assets.instance.pistolTrailMaterial;
        else
            trail.sharedMaterial = Assets.instance.rifleTrailMaterial;
    }

    public override void damaged(int damage, BaseEntity attacker)
    {
        audioSource.clip = Assets.instance.playerHitSound;
        audioSource.volume = Assets.instance.playerHitSoundVolume * GC.SOUND_VOLUME;
        audioSource.Play();
        health -= damage;

        if (health <= 0)
        {
            if (!GC.INFINITE_HP)
            {
                audioSource.clip = Assets.instance.playerDeathSound;
                audioSource.volume = Assets.instance.playerDeathSoundVolume * GC.SOUND_VOLUME;
                audioSource.Play();

                disableCrouch();
                gameManager.onPlayerDeath(true);
            }
        }

        gameManager.onPlayerDamaged();
    }
}
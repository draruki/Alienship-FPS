using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Assets : MonoBehaviour 
{
    public static Assets instance;

    // prefabs
    public GameObject decalPrefab;
    public GameObject particlePrefab;
    public GameObject bulletPrefab;

    // music
    public AudioClip mainMenuMusic;
    [Range(0f, 1f)] public float mainMenuMusicVolume = 1.0f;
    public AudioClip gameMusic;
    [Range(0f, 1f)] public float gameMusicVolume = 1.0f;

    // sounds
    public AudioClip menuSelectSound;
    [Range(0f, 1f)] public float menuSelectSoundVolume = 1.0f;
    public AudioClip menuMoveSound;
    [Range(0f, 1f)] public float menuMoveSoundVolume = 1.0f;
    public AudioClip pickupSound;
    [Range(0f, 1f)] public float pickupSoundVolume = 1.0f;
    public AudioClip pistolSound;
    [Range(0f, 1f)] public float pistolSoundVolume = 1.0f;
    public AudioClip playerDeathSound;
    [Range(0f, 1f)] public float playerDeathSoundVolume = 1.0f;
    public AudioClip playerHitSound;
    [Range(0f, 1f)] public float playerHitSoundVolume = 1.0f;
    public AudioClip rifleSound;
    [Range(0f, 1f)] public float rifleSoundVolume = 1.0f;
    public AudioClip leverSound;
    [Range(0f, 1f)] public float leverSoundVolume = 1.0f;
    public AudioClip shotgunSound;
    [Range(0f, 1f)] public float shotgunSoundVolume = 1.0f;
    public AudioClip ff7DeathSound;
    [Range(0f, 1f)] public float ff7DeathSoundVolume = 1.0f;

    // materials
    public Material shotgunTrailMaterial;
    public Material flashMaterial;
    public Material pistolTrailMaterial;
    public Material pistolMuzzleMaterial;
    public Material deathMaterial;
    public Material rifleMuzzleMaterial;
    public Material rifleTrailMaterial;
    public Material shotgunMuzzleMaterial;

    // images
    public Sprite singleAmmoSprite;
    public Sprite autoAmmoSprite;
    public Sprite spreadAmmoSprite;
    public Sprite shotgunCrosshairSprite;
    public Sprite pistolCrosshairSprite;
    public Sprite rifleCrosshairSprite;

    void Awake()
    {
        Assets.instance = this;
    }
}
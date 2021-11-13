using UnityEngine;
using System.Collections;

public class Weapon
{
    public enum TYPE {SINGLE, SPREAD, AUTO};
    
    public string name;
    public int damage;
    public Weapon.TYPE type;
    public float cooldown;
    public float distance;
    public float spread;
    public float shootVolume;
    public float delay;
    public GameObject model;
    public AudioClip shootSound;
    public Vector3 muzzleFlashPosition;
    public Material muzzleFlashMaterial;

    public Weapon() {}
}

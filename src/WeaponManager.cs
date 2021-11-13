using UnityEngine;
using System.Collections;

public class WeaponManager
{
    public enum WEAPON {Pistol, Shotgun, Rifle};

    public static Weapon pistol;
    public static Weapon rifle;
    public static Weapon shotgun;

    public static void initialize()
    {
        // pistol
        pistol = new Weapon();
        pistol.name = "Light Pistol";
        pistol.type = Weapon.TYPE.SINGLE;
        pistol.distance = 120f;
        pistol.damage = 11;
        pistol.cooldown = 0.30f;
        pistol.muzzleFlashMaterial = Assets.instance.pistolMuzzleMaterial;
        pistol.delay = 0.04f;
        pistol.model = Assets.instance.pistol;
        pistol.shootSound = Assets.instance.pistolSound;
        pistol.shootVolume = Assets.instance.pistolSoundVolume;
        pistol.muzzleFlashPosition = new Vector3(0, -0.303f, 0.831f);

        // rifle
        rifle = new Weapon();
        rifle.name = "Pulse Rifle";
        rifle.type = Weapon.TYPE.AUTO;
        rifle.distance = 160f;
        rifle.damage = 6;
        rifle.cooldown = 0.1f;
        rifle.muzzleFlashMaterial = Assets.instance.rifleMuzzleMaterial;
        rifle.delay = 0.04f;
        rifle.model = Assets.instance.rifle;
        rifle.shootSound = Assets.instance.rifleSound;
        rifle.shootVolume = Assets.instance.rifleSoundVolume;
        rifle.muzzleFlashPosition = new Vector3(0, -0.303f, 1.7334f);

        // shotgun
        shotgun = new Weapon();
        shotgun.name = "Blaster";
        shotgun.type = Weapon.TYPE.SPREAD;
        shotgun.distance = 10f;
        shotgun.damage = 3;
        shotgun.cooldown = 0.8f;
        shotgun.muzzleFlashMaterial = Assets.instance.shotgunMuzzleMaterial;
        shotgun.delay = 0.08f;
        shotgun.model = Assets.instance.shotgun;
        shotgun.shootSound = Assets.instance.shotgunSound;
        shotgun.shootVolume = Assets.instance.shotgunSoundVolume;
        shotgun.muzzleFlashPosition = new Vector3(0, -0.3503f, 1.107f);
        shotgun.spread = 0.46f;
    }
}
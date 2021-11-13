using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour 
{
    private static Transform parent;

    // blood FX
    private static float bloodLifetime = 2f;
    public int particleStartCount = 20;
    private static List<Particle> list = new List<Particle>();

    // bullet FX
    private static float pistolBulletLifetime = 0.3f;
    public int bulletStartCount = 35;
    private static List<Projectile> pistolBulletList = new List<Projectile>();

    // decal FX
    private static float decalLifetime = 3600f;
    public int decalStartCount = 100;
    private static List<Decal> decalList = new List<Decal>();

    void Start () 
    {
        ParticleManager.parent = transform;

        /*
        for (int i = 0; i < particleStartCount; ++i)
        {
            ParticleManager.instantiateParticle();
        }
        */
        for (int i = 0; i < bulletStartCount; ++i)
        {
            ParticleManager.instantiatePistolBullet();
        }
        for (int i = 0; i < decalStartCount; ++i)
        {
            ParticleManager.instantiateDecal();
        }
    }

    private static void instantiateParticle()
    {
        Particle p = Instantiate(Assets.instance.particlePrefab).GetComponent<Particle>();
        p.transform.parent = ParticleManager.parent;
        list.Add(p);
        p.gameObject.SetActive(false);
    }

    private static void instantiatePistolBullet()
    {
        Projectile p = Instantiate(Assets.instance.pistolBulletPrefab).GetComponent<Projectile>();
        p.transform.parent = ParticleManager.parent;
        pistolBulletList.Add(p);
        p.gameObject.SetActive(false);
    }

    private static void instantiateDecal()
    {
        Decal d = Instantiate(Assets.instance.decalPrefab).GetComponent<Decal>();
        d.transform.parent = ParticleManager.parent;
        decalList.Add(d);
        d.gameObject.SetActive(false);
    }

    public static void emitBlood(Vector3 position, Color color)
    {
        return;
        
        int emitCount = 5;
        for (int i = 0; i < list.Count && emitCount != 0; ++i)
        {
            if (list[i].gameObject.activeInHierarchy == false)
            {
                list[i].lifetime = bloodLifetime;
                list[i].gameObject.SetActive(true);
                list[i].transform.position = position;
                emitCount--;
            }
        }
        if (emitCount == 2)
        {
            ParticleManager.instantiateParticle();
            ParticleManager.instantiateParticle();
        }
    }

    public static void shootPistolBullet(Vector3 position, Vector3 direction)
    {
        bool found = false;
        for (int i = 0; i < pistolBulletList.Count; ++i)
        {
            if (pistolBulletList[i].gameObject.activeInHierarchy == false)
            {
                pistolBulletList[i].gameObject.SetActive(true);
                pistolBulletList[i].transform.position = position;
                pistolBulletList[i].lifetime = pistolBulletLifetime;
                pistolBulletList[i].setDirection(direction);
                found = true;
                break;
            }
        }
        if (!found)
        {
            ParticleManager.instantiatePistolBullet();
            ParticleManager.instantiatePistolBullet();
        }
    }

    public static void plasterDecal(Vector3 position, Vector3 normal)
    {
        Decal d = decalList[Random.Range(0, decalList.Count - 1)];
        for (int i = 0; i < decalList.Count; ++i)
        {
            if (decalList[i].gameObject.activeInHierarchy == false)
            {
                d = decalList[i];
                break;
            }
        }

        d.gameObject.SetActive(true);
        d.setDirectionNormal(normal);
        d.lifetime = decalLifetime;
        d.transform.position = position;
    }

    public static void clearDecals()
    {

    }

    void Update()
    {
        /*
        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].gameObject.activeInHierarchy)
            {
                if (list[i].lifetime <= 0)
                {
                    list[i].gameObject.SetActive(false);
                }
            }
        }
        */

        for (int i = 0; i < pistolBulletList.Count; ++i)
        {
            if (pistolBulletList[i].gameObject.activeInHierarchy)
            {
                if (pistolBulletList[i].lifetime <= 0)
                {
                    pistolBulletList[i].gameObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < decalList.Count; ++i)
        {
            if (decalList[i].gameObject.activeInHierarchy)
            {
                if (decalList[i].lifetime <= 0)
                {
                    decalList[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
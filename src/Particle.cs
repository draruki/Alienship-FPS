using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour
{
    [HideInInspector]
    public float lifetime;
    private const double gravity = -15f;
    private Vector3 velocity;
    private Camera gameCamera;

    void Start()
    {
        velocity = Vector3.zero;
        gameCamera = GC.gameCamera;
    }

    void Update()
    {
        if (GC.PAUSED)
            return;

        lifetime -= Time.deltaTime;
        transform.Translate(0, -2 * Time.deltaTime, 0);
        if (gameCamera != null)
            transform.LookAt(transform.position + gameCamera.transform.rotation * Vector3.forward, gameCamera.transform.rotation * Vector3.up);
    }
}


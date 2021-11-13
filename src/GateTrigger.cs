using UnityEngine;
using System.Collections;

public class GateTrigger : MonoBehaviour 
{
    private bool triggered;
    private AudioSource audioSource;
    public GameObject buttonModel;
    public Gate gate;
    public float buttonPressY = 0.1f;

	void Start () 
    {
        triggered = false;
        audioSource = GetComponent<AudioSource>();
	}

    private void activate()
    {
        audioSource.clip = Assets.instance.leverSound;
        audioSource.Play();
        audioSource.volume = Assets.instance.leverSoundVolume * GC.SOUND_VOLUME;
        triggered = true;
        gate.open();

        if (buttonModel != null)
        {
            buttonModel.transform.Translate(0, -buttonPressY, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            PlayerEntity player = other.gameObject.GetComponent<PlayerEntity>();

            if (player != null)
            {
                activate();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedZombie : Enemy
{
    private Vector3 initialRotation;
    public GameObject Shield;
    public bool IsShieldBroken = false;

    public GameObject SpawnAudioCue;

    protected override void Start()
    {
        base.Start();
        initialRotation = Shield.transform.localRotation.eulerAngles;

        float audioDelay = Random.Range(0f, 4f);
        Invoke("PlayAudioCue", audioDelay); // Losowy delay 0-4 sekundy
    }

    void PlayAudioCue()
    {
        if (SpawnAudioCue != null)
        {
            GameObject audio = Instantiate(SpawnAudioCue, transform.position, Quaternion.identity);
            Destroy(audio, 5f);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            Shield.transform.localRotation = Quaternion.Euler(-15, -15, -156);
        }
    }
    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.gameObject.layer == 10)
        {
            Shield.transform.localRotation = Quaternion.Euler(initialRotation);
        }
    }
}

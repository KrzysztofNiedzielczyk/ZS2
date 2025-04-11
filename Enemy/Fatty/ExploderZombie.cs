using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderZombie : Enemy
{
    //death explosion objects
    public GameObject ExplosionyAudioObj;
    public GameObject BuffEffect;

    //explosion parameters
    public float Radius = 15f;
    public float ExplosionDamage = 50f;
    public float ExplosionBuff = 50f;

    //camera shake
    public CinemachineImpulseSource CameraImpulseSource;

    public GameObject SpawnAudioCue;

    protected override void Start()
    {
        base.Start();
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

    public override void Die()
    {
        Instantiate(DieGibbletsFX, transform.position + new Vector3(0, 1.2f, 0), Quaternion.identity);
        Instantiate(DieBloodExplosionFX, transform.position + new Vector3(0, 1.2f, 0), transform.rotation);
        DeathExplosion();
        EventManager.OnEnemyDied(gameObject);
        Destroy(gameObject);
    }

    void DeathExplosion()
    {
        if(ExplosionyAudioObj != null)
        {
            GameObject _explosionAudio = Instantiate(ExplosionyAudioObj, transform.position, Quaternion.identity);
            Destroy(_explosionAudio, 5f);
        }
        
        // shake the camera
        CameraShakeManager.Instance.CameraShake(CameraImpulseSource);


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out Enemy _enemy))
            {
                if(_enemy.gameObject != this.gameObject)
                {
                    _enemy.Health += ExplosionBuff;
                    GameObject _buffExplosion = Instantiate(BuffEffect, _enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity, _enemy.transform);
                    Destroy(_buffExplosion, 3f);
                }
            }
            else if (hitCollider.TryGetComponent(out BaseBarricade _baseBarricade))
            {
                _baseBarricade.TakeDamage(ExplosionDamage);
            }
        }
    }
}

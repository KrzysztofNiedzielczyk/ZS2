using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerExplosionProjectile : MonoBehaviour
{
    public float Damage;
    public float Radius;
    public float Time;

    public GameObject ExplosionFX;

    public List<GameObject> ExplosionAudio;

    public CinemachineImpulseSource cameraImpulseSource;

    public Rigidbody Rb;

    private void Start()
    {
        StartCoroutine(ExplosionTimer());
        Rb = GetComponent<Rigidbody>();
    }

    public virtual IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(Time);

        GameObject _explosion = Instantiate(ExplosionFX, transform.position, Quaternion.Euler(0, 0, 0));

        int _index = Random.Range(0, ExplosionAudio.Count);
        GameObject _explosionAudio = Instantiate(ExplosionAudio[_index], transform.position, Quaternion.identity);

        Destroy(_explosion, 10f);
        Destroy(_explosionAudio, 10f);


        ExplosionEffect();

        // shake the camera
        CameraShakeManager.Instance.CameraShake(cameraImpulseSource);
        Destroy(gameObject);
    }

    public virtual void ExplosionEffect()
    {
        Collider[] _hitColliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (var hitCollider in _hitColliders)
        {
            if (hitCollider.transform.TryGetComponent(out IDamageable damageable))
            {
                float _distance = Vector3.Distance(transform.position, hitCollider.transform.position);

                if (_distance <= Radius / 3)
                {
                    damageable.TakeDamage(Damage);
                }
                else if ((_distance > Radius / 3) && (_distance <= Radius / 1.5f))
                {
                    damageable.TakeDamage(Damage / 2);
                }
                else
                {
                    damageable.TakeDamage(Damage / 4);
                }
            }
        }
    }
}

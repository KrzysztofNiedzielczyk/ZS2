using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImpactExplosionProjectile : MonoBehaviour
{
    public float Damage;
    public float Radius;

    public int[] layers = { 0, 6, 7 };

    public GameObject ExplosionFX;
    public GameObject Indicator;

    public GameObject SpawnedIndicator;

    public List<GameObject> ExplosionAudio;

    public CinemachineImpulseSource cameraImpulseSource;

    public virtual void OnCollisionEnter(Collision collision)
    {
        int _collisionLayer = collision.gameObject.layer;

        if (layers.Contains(_collisionLayer))
        {
            GameObject _explosion = Instantiate(ExplosionFX, transform.position, Quaternion.Euler(0, 0, 0));

            int _index = Random.Range(0, ExplosionAudio.Count);
            if(ExplosionAudio.Count > 0)
            {
                GameObject _explosionAudio = Instantiate(ExplosionAudio[_index], transform.position, Quaternion.identity);
                Destroy(_explosionAudio, 10f);
            }

            Destroy(_explosion, 20f);


            Collider[] _hitColliders = Physics.OverlapSphere(transform.position, Radius);
            foreach (var hitCollider in _hitColliders)
            {
                if (hitCollider.transform.TryGetComponent(out IDamageable damageable))
                {
                    if (Vector3.Distance(transform.position, hitCollider.transform.position) <= Radius / 3)
                    {
                        damageable.TakeDamage(Damage);
                    }
                    else if ((Vector3.Distance(transform.position, hitCollider.transform.position) > Radius / 3) && (Vector3.Distance(transform.position, hitCollider.transform.position) <= Radius / 1.5f))
                    {
                        damageable.TakeDamage(Damage / 2);
                    }
                    else
                    {
                        damageable.TakeDamage(Damage / 4);
                    }
                }

                Destroy(gameObject);
            }

            // shake the camera
            CameraShakeManager.Instance.CameraShake(cameraImpulseSource);

            if (SpawnedIndicator != null)
            {
                Destroy(SpawnedIndicator);
            }
        }

        if (SpawnedIndicator != null)
        {
            Destroy(SpawnedIndicator, 5);
        }

        Destroy(gameObject, 10);
    }
}

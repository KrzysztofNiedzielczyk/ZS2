using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float damage;
    public Rigidbody rb;
    public GameObject impactEffect;

    public Coroutine releaseCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void FixedUpdate()
    {
        rb.linearVelocity = transform.up * speed;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable hit))
        {
            hit.TakeDamage(damage);

            if (releaseCoroutine != null)
            {
                StopCoroutine(releaseCoroutine);
            }

            //leave hit impact effect
            LeaveImpactEffect(collision.contacts[0]);

            Destroy(gameObject);
        }
        else
        {
            if (releaseCoroutine != null)
            {
                StopCoroutine(releaseCoroutine);
            }

            //leave hit impact effect
            LeaveImpactEffect(collision.contacts[0]);

            Destroy(gameObject);
        }
    }

    public void LeaveImpactEffect(ContactPoint hit)
    {
        if (impactEffect != null)
        {
            GameObject impactGamObj = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGamObj, 2f);
        }
    }
}


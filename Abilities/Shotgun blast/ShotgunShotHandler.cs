using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunShotHandler : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject ImpactEffect;
    public float Speed = 10f;
    public float Damage = 40f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.up * Speed * Time.deltaTime, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Enemy enemy))
        {
            ParticleSystem _bloodSplash = Instantiate(enemy.BloodSplashFX, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
            Destroy(_bloodSplash, 2f);

            if (enemy.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(Damage);
            }
        }
        else
        {
            Instantiate(ImpactEffect, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}

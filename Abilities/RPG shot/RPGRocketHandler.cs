using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGRocketHandler : ImpactExplosionProjectile
{
    private Rigidbody rb;
    public Transform RocketTrail;
    public float Speed = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 30f, ForceMode.VelocityChange);
        RocketTrail.parent = null;
    }

    private void Update()
    {
        if(gameObject !=  null)
        {
            RocketTrail.transform.position = transform.position;
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.forward * Speed * Time.deltaTime, ForceMode.Impulse);
    }
}

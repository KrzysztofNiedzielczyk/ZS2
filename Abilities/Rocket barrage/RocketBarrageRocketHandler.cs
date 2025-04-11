using System;
using System.Collections;
using UnityEngine;

public class RocketBarrageRocketHandler : ImpactExplosionProjectile
{
    public float RotationSpeed;
    public Vector3 TargetPosition;
    public float ActivationDelay = 0.5f;
    private bool activated = false;
    private Coroutine initialActivationCoroutine;
    public float Speed = 10f;
    public float InitialSpeed = 20f;
    public Rigidbody Rb;

    private void OnEnable()
    {
        initialActivationCoroutine = StartCoroutine(InitialActivationDelay());
        Rb.linearVelocity = Vector3.zero;
        InitialThrust();
    }

    private void OnDisable()
    {
        activated = false;
        StopCoroutine(initialActivationCoroutine);
    }

    public void FixedUpdate()
    {
        if (activated)
        {
            Rotate();
            Rb.linearVelocity = transform.forward * Speed;
        }
    }

    IEnumerator InitialActivationDelay()
    {
        yield return new WaitForSeconds(ActivationDelay);
        activated = true;
    }

    void Rotate()
    {
        //find the vector pointing from our position to the target
        Vector3 _direction = (TargetPosition - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);

        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
    }

    public void InitialThrust()
    {
        Rb.AddForce(transform.forward * InitialSpeed, ForceMode.VelocityChange);
    }
}

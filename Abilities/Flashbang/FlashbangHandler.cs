using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashbangHandler : TimerExplosionProjectile
{
    public float StunDuration;
    public GameObject StunEffect;

    private void OnCollisionEnter(Collision collision)
    {
        Rb.linearVelocity /= 2;
    }

    public override void ExplosionEffect()
    {
        Collider[] _hitColliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (var hitCollider in _hitColliders)
        {
            if (hitCollider.tag != "Player" && hitCollider.transform.TryGetComponent(out IStunnable stunnable))
            {
                stunnable.GetStunned(StunDuration, StunEffect);
            }
        }
    }
}

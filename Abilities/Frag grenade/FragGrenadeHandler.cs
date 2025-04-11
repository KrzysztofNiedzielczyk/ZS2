using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragGrenadeHandler : TimerExplosionProjectile
{
    private void OnCollisionEnter(Collision collision)
    {
        Rb.linearVelocity /= 2;
    }
}

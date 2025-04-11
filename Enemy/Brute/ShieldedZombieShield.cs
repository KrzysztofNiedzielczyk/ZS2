using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedZombieShield : MonoBehaviour, IDamageable, IPenetrable
{
    public float MaxHealth = 100;
    public float Health = 100;
    public bool IsBroken = false;
    public ParticleSystem BreakFX;

    [field: SerializeField]
    public bool IsPenetrable { get; set; } = false;

    public virtual void TakeDamage(float amount)
    {
        Health -= amount;

        if (IsBroken == false && Health <= 0)
        {
            IsBroken = true;
            Break();
        }
    }

    public virtual void Break()
    {
        Instantiate(BreakFX, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        Destroy(gameObject);
    }
}

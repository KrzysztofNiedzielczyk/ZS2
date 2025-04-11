using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    public float DamageValue = 1f;
    public float DamageRate = 0.1f;

    public virtual float TakeDamageOverTime()
    {
        float _damage = DamageValue * DamageRate * Time.deltaTime;

        return _damage;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(TakeDamageOverTime());
        }
    }
}

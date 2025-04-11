using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnergyPulseHandler : Ability
{
    private bool fireing = false;
    public float Radius = 20f;
    public float Power = 1000f;
    public float UpwardPower = 100f;
    public float Damage = 0f;
    public Transform CenterOfPulse;
    public ParticleSystem EnergyBlastVFX;
    public List<AudioSource> ShotSFX = new List<AudioSource>();

    public float StunDuration;
    public GameObject StunEffect;
    public GameObject PulsePusher;

    private void Update()
    {
        Fireing();
    }

    public override void OnAbilityActivation(InputAction.CallbackContext context)
    {
        //get if fire input was cancelled or else allow fireing
        if (context.performed)
        {
            fireing = true;
        }
        else
        {
            fireing = false;
        }
    }

    void Fireing()
    {
        //if input is stopped then do not execute code
        if (IsAvailable && fireing)
        {
            fireing = false;
            Fire();

            //send event to artillery timer to reset
            //EventManager.OnShotgunCalled();
        }
    }

    public void Fire()
    {
        StartCoroutine(AbilityDelay());
        ResourcesManager.Instance.Ammunition -= ResourceCost;

        StartCoroutine(PushForceEnabler());

        EnergyBlastVFX.Play();

        if (ShotSFX.Count > 0)
        {
            int _randomIndex = Random.Range(0, ShotSFX.Count - 1);
            ShotSFX[_randomIndex].Play();
        }

        Vector3 explosionPos = CenterOfPulse.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, Radius);

        foreach (Collider hit in colliders)
        {
            if(hit.gameObject.layer == 9)
            {
                if (hit.transform.TryGetComponent(out IStunnable stunnable))
                {
                    stunnable.GetStunned(StunDuration, StunEffect);
                }

                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddExplosionForce(Power, explosionPos, Radius, UpwardPower);
                }

                if (hit.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(Damage);
                }
            }
        }
    }

    IEnumerator PushForceEnabler()
    {
        PulsePusher.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        PulsePusher.SetActive(false);
    }
}

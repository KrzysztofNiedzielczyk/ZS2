using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeavySniperShotHandler : Ability
{
    private bool fireing = false;
    private int layerMask;
    public float Range = 500f;
    public float Damage = 300f;
    public int PenetrationLevel = 100;
    public Transform Muzzle;
    public ParticleSystem MuzzleFlash;
    public GameObject ImpactEffect;
    public TrailRenderer GunBulletTrail;
    public List<AudioSource> ShotSFX = new List<AudioSource>();

    private void Start()
    {
        layerMask |= (1 << 0);
        layerMask |= (1 << 6);
        layerMask |= (1 << 7);
        layerMask |= (1 << 8);
        layerMask |= (1 << 9);
    }

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

        MuzzleFlash.Play();

        int _randomIndex = Random.Range(0, ShotSFX.Count - 1);
        ShotSFX[_randomIndex].Play();

        Vector3 _forwardVector = Vector3.forward;
        _forwardVector = Muzzle.rotation * _forwardVector;

        // detect multiple objects in Raycast line
        RaycastHit[] hits = Physics.RaycastAll(Muzzle.position, _forwardVector, Range, layerMask, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        if (hits != null)
        {
            int _penetrationNumber;

            if (hits.Length >= PenetrationLevel)
            {
                _penetrationNumber = PenetrationLevel;
            }
            else
            {
                _penetrationNumber = hits.Length;
            }

            for (int i = 0; i < _penetrationNumber; i++)
            {
                // spawn bullet trail
                TrailRenderer _trail = Instantiate(GunBulletTrail, Muzzle.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(_trail, hits[i]));

                if (hits[i].transform.TryGetComponent(out Enemy enemy))
                {
                    ParticleSystem _bloodSplash = Instantiate(enemy.BloodSplashFX, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                    Destroy(_bloodSplash, 2f);

                    if (enemy.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage(Damage);
                    }
                }
                else
                {
                    Instantiate(ImpactEffect, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                }

                // detect if object is penetrable and if yes do not shoot through it by breaking the loop
                if (hits[i].transform.TryGetComponent(out IPenetrable penetrable))
                {
                    if (penetrable.IsPenetrable == false)
                    {
                        break;
                    }
                }
            }
        }
    }

    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 _startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(_startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;

        Destroy(trail.gameObject, trail.time);
    }
}

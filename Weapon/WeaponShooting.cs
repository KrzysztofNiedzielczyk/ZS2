using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponShooting : MonoBehaviour
{
    private bool fireing = false;
    private int layerMask;
    public Weapon Weapon;
    private float nextTimeToFire = 0.1f;
    public Player Player;
    public WeaponRotation WeaponRotation;
    private bool isReloading = false;
    private int reloadingAudioIndex;
    [SerializeField] private int ammoAmount;
    private float sleepDebtPenalty = 1f; // Modyfikator karny

    private void Start()
    {
        layerMask |= (1 << 0);
        layerMask |= (1 << 6);
        layerMask |= (1 << 7);
        layerMask |= (1 << 8);
        layerMask |= (1 << 9);
        layerMask |= (1 << 13);

        Player = GetComponent<Player>();
        WeaponRotation = GetComponent<WeaponRotation>();
        ammoAmount = Weapon.AmmoAmount;
    }

    private void Update()
    {
        UpdateSleepPenalty(); // Nowa metoda
        Fireing();
        Reload();
    }

    private void UpdateSleepPenalty()
    {
        switch (ResourcesManager.Instance.CurrentSleepDebtLevel)
        {
            case ResourcesManager.SleepDebtLevel.Rested:
                sleepDebtPenalty = 1f; // Brak kar
                break;
            case ResourcesManager.SleepDebtLevel.Tired:
                sleepDebtPenalty = 0.9f; // -10% szybkoœci
                break;
            case ResourcesManager.SleepDebtLevel.Exhausted:
                sleepDebtPenalty = 0.75f; // -25% szybkoœci
                break;
            case ResourcesManager.SleepDebtLevel.Critical:
                sleepDebtPenalty = 0.5f; // -50% szybkoœci
                break;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        //get if fire input was cancelled or else allow fireing
        if (context.canceled)
        {
            fireing = false;
        }
        else
        {
            fireing = true;
        }
    }

    void Fireing()
    {
        if (isReloading == false && Player.Stunned == false && fireing && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1f / Weapon.FireRate) / sleepDebtPenalty; // Kara do szybkoœci strzelania
            Fire();
        }
    }

    public void Fire()
    {
        if (Weapon.MuzzleFlash[0].isPlaying)
        {
            Weapon.MuzzleFlash[1].Play();
        }
        else if (Weapon.MuzzleFlash[1].isPlaying)
        {
            Weapon.MuzzleFlash[2].Play();
        }
        else
        {
            Weapon.MuzzleFlash[0].Play();
        }

        ResourcesManager.Instance.Ammunition--;
        ammoAmount--;

        int _randomIndex = Random.Range(0, Weapon.ShootAudio.Count-1);
        Weapon.ShootAudio[_randomIndex].Play();

        Vector3 _forwardVector = Vector3.forward;
        float _deviation = Random.Range(0f, 0.7f);
        float _angle = Random.Range(0f, 360f);
        _forwardVector = Quaternion.AngleAxis(_deviation, Vector3.up) * _forwardVector;
        _forwardVector = Quaternion.AngleAxis(_angle, Vector3.forward) * _forwardVector;
        _forwardVector = Weapon.Muzzle.rotation * _forwardVector;

        // detect multiple objects in Raycast line
        RaycastHit[] hits = Physics.RaycastAll(Weapon.Muzzle.position, _forwardVector, Weapon.Range, layerMask, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        if (hits != null)
        {
            int _penetrationNumber;

            if (hits.Length >= Weapon.PenetrationLevel)
            {
                _penetrationNumber = Weapon.PenetrationLevel;
            }
            else
            {
                _penetrationNumber = hits.Length;
            }

            for (int i = 0; i < _penetrationNumber; i++)
            {
                // spawn bullet trail
                TrailRenderer _trail = Instantiate(Weapon.GunBulletTrail, Weapon.Muzzle.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(_trail, hits[i]));

                if (hits[i].collider.CompareTag("Armor"))
                {
                    Instantiate(Weapon.ImpactEffectMetal, hits[i].point, Quaternion.LookRotation(hits[i].normal));

                    if (hits[i].collider.TryGetComponent(out ShieldedZombieShield shieldedZombieShield))
                    {
                        shieldedZombieShield.TakeDamage(Weapon.Damage);

                        if (hits[i].collider.TryGetComponent(out IPenetrable _penetrable1))
                        {
                            if (_penetrable1.IsPenetrable == false)
                            {
                                break;
                            }
                        }
                    }
                }
                else if (hits[i].transform.gameObject.layer == 13)
                {
                    if (hits[i].transform.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage(Weapon.Damage);
                    }
                }
                else if (hits[i].transform.TryGetComponent(out Enemy _enemy))
                {
                    ParticleSystem _bloodSplash = Instantiate(_enemy.BloodSplashFX, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                    Destroy(_bloodSplash, 2f);

                    if (_enemy.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage(Weapon.Damage);
                    }
                }
                else
                {
                    Instantiate(Weapon.ImpactEffect, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                }

                // detect if object is penetrable and if yes do not shoot through it by breaking the loop
                if(hits[i].transform.TryGetComponent(out IPenetrable _penetrable2))
                {
                    if(_penetrable2.IsPenetrable == false)
                    {
                        break;
                    }
                }
            }
        }
    }

    void Reload()
    {
        // TO-DO: add reload button
        if (isReloading == false && ammoAmount <= 0)
        {
            isReloading = true;
            StartCoroutine(ReloadTime());
        }

        if (isReloading)
        {
            int index = Random.Range(0, Weapon.WeaponFoleySFXList.Count);

            if (Weapon.WeaponFoleySFXList[reloadingAudioIndex].isPlaying)
            {
                return;
            }

            Weapon.WeaponFoleySFXList[index].Play();
            reloadingAudioIndex = index;
        }
    }

    IEnumerator ReloadTime()
    {
        float reloadingTime = (Weapon.ReloadTime / ResourcesManager.Instance.AmmunitionCurrentBonus) / sleepDebtPenalty; // Kara do prze³adowania

        while (reloadingTime > 0)
        {
            reloadingTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (reloadingTime <= 0)
        {
            isReloading = false;
            ammoAmount = Weapon.AmmoAmount;
            int index = Random.Range(0, Weapon.WeaponReloadSFXList.Count);
            Weapon.WeaponReloadSFXList[index].Play();
            yield break;
        }
    }

    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 _startPosition = trail.transform.position;

        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(_startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;

        Destroy(trail.gameObject, trail.time);
    }
}

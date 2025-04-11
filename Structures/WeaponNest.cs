using SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponNest : MonoBehaviour
{
    public Transform GunPosition;
    public Transform Muzzle;
    public FOVCollider FOVCollider;
    private int layerMask;

    public float Damage = 9f;
    public float RotationSpeed = 5f;
    public float FireRange = 25f;
    public float FireRate = 8f;
    public float FireHorizontalAngle = 90f;
    public float FireVerticalAngle = 90f;
    [SerializeField] private List<GameObject> targetsList = new List<GameObject>();
    private Transform nearestValidEnemy;
    private float nextTimeToFire = 0f;
    public int PenetrationLevel = 2;

    public List<ParticleSystem> MuzzleFlash = new List<ParticleSystem>();
    public List<AudioSource> ShootAudio = new List<AudioSource>();
    public TrailRenderer GunBulletTrail;
    public GameObject ImpactEffect;
    public GameObject ImpactEffectMetal;

    private void OnEnable()
    {
        EventManager.EnemyDied += OnEnemyDied;
    }

    private void OnDisable()
    {
        EventManager.EnemyDied -= OnEnemyDied;
    }

    public virtual void OnEnemyDied(GameObject _enemy)
    {
        if (targetsList.Contains(_enemy))
        {
            targetsList.Remove(_enemy);
        }
    }

    private void Awake()
    {
        FOVCollider.transform.position = GunPosition.position;
        FOVCollider.Length = FireRange;
        FOVCollider.FOVAngle = FireHorizontalAngle;
        FOVCollider.ElevationAngle = FireVerticalAngle;
        FOVCollider.CreateCollider();
    }

    private void Start()
    {
        layerMask |= (1 << 0);
        layerMask |= (1 << 6);
        layerMask |= (1 << 7);
        layerMask |= (1 << 8);
        layerMask |= (1 << 9);
        layerMask |= (1 << 13);

        //StartCoroutine(DetectionCheck());
    }

    private void Update()
    {
        GetClosestEnemy();
        RotateWeapon();
        FireAtEnemy();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!targetsList.Contains(other.gameObject) && other.gameObject.layer == 9)
        {
            targetsList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            targetsList.Remove(other.gameObject);

            if(nearestValidEnemy == other.transform)
            {
                nearestValidEnemy = null;
            }
        }
    }

    /*IEnumerator DetectionCheck()
    {
        while (true)
        {
            GetClosestEnemy();
            yield return new WaitForSeconds(0.2f);
        }
    }*/

    void FireAtEnemy()
    {
        if (targetsList.Count > 0 && nearestValidEnemy != null && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / FireRate;

            if (MuzzleFlash[0].isPlaying)
            {
                MuzzleFlash[1].Play();
            }
            else if (MuzzleFlash[1].isPlaying)
            {
                MuzzleFlash[2].Play();
            }
            else
            {
                MuzzleFlash[0].Play();
            }

            int _randomIndex = Random.Range(0, ShootAudio.Count - 1);
            ShootAudio[_randomIndex].Play();

            Vector3 _forwardVector = Vector3.forward;
            float _deviation = Random.Range(0f, 0.7f);
            float _angle = Random.Range(0f, 360f);
            _forwardVector = Quaternion.AngleAxis(_deviation, Vector3.up) * _forwardVector;
            _forwardVector = Quaternion.AngleAxis(_angle, Vector3.forward) * _forwardVector;
            _forwardVector = Muzzle.rotation * _forwardVector;

            // detect multiple objects in Raycast line
            RaycastHit[] hits = Physics.RaycastAll(Muzzle.position, _forwardVector, 1000f, layerMask, QueryTriggerInteraction.Collide);
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

                    if (hits[i].collider.CompareTag("Armor"))
                    {
                        Instantiate(ImpactEffectMetal, hits[i].point, Quaternion.LookRotation(hits[i].normal));

                        if (hits[i].collider.TryGetComponent(out ShieldedZombieShield shieldedZombieShield))
                        {
                            shieldedZombieShield.TakeDamage(Damage);

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
                            damageable.TakeDamage(Damage);
                        }
                    }
                    else if (hits[i].transform.TryGetComponent(out Enemy _enemy))
                    {
                        ParticleSystem _bloodSplash = Instantiate(_enemy.BloodSplashFX, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                        Destroy(_bloodSplash, 2f);

                        if (_enemy.TryGetComponent(out IDamageable damageable))
                        {
                            damageable.TakeDamage(Damage);
                        }
                    }
                    else
                    {
                        Instantiate(ImpactEffect, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                    }

                    // detect if object is penetrable and if yes do not shoot through it by breaking the loop
                    if (hits[i].transform.TryGetComponent(out IPenetrable _penetrable2))
                    {
                        if (_penetrable2.IsPenetrable == false)
                        {
                            break;
                        }
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

    void ReloadWeapon()
    {

    }

    void RotateWeapon()
    {
        Quaternion _lookRotation;
        Vector3 _direction;

        if (nearestValidEnemy != null)
        {
            if(nearestValidEnemy.TryGetComponent(out IAimable aimable))
            {
                _direction = (aimable.AimTransform.position - GunPosition.position).normalized;
                _lookRotation = Quaternion.LookRotation(_direction);
                GunPosition.rotation = Quaternion.Slerp(GunPosition.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
            }
        }
    }

    void GetClosestEnemy()
    {
        if(targetsList.Count > 0)
        {
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (GameObject potentialTarget in targetsList)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    nearestValidEnemy = potentialTarget.transform;
                }
            }
        }
    }
}

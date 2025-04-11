using Pathfinding;
using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable, IPenetrable, IAimable, IStunnable
{
    public float MaxHealth;
    public float Health;
    public float Speed;
    public float Damage;
    public int DieGibletsNumber;
    public bool IsTouchingGround;
    public bool IsDead = false;
    public GameObject attackTarget;
    public Rigidbody Rigidbody;
    public RichAI RichAI;
    public Seeker Seeker;
    public AIDestinationSetter AIDestinationSetter;
    public ParticleSystem BloodSplashFX;
    public ParticleSystem DieGibbletsFX;
    public ParticleSystem DieBloodExplosionFX;
    public Animator Animator;
    private Coroutine stunnedCoroutine;

    [Header("Anti-Stacking Settings")]
    public float SeparationForce = 2f; // Siła odpychania między zombie
    public float GroundingForce = 50f; // Siła przyciągania do ziemi

    [field: SerializeField]
    public bool IsPenetrable { get; set; } = true;
    [field: SerializeField]
    public Transform AimTransform { get; set; }

    protected virtual void Awake()
    {
        Rigidbody.constraints = RigidbodyConstraints.FreezeRotation; // Zamroź rotację, aby uniknąć przewracania
    }

    protected virtual void Start()
    {
        Health = MaxHealth;
        RichAI.maxSpeed = Speed;
        UpdateToNearestDestinationTarget();
    }

    protected virtual void OnEnable()
    {
        UpdateToNearestDestinationTarget();
    }

    private void FixedUpdate()
    {
        if (!IsTouchingGround && !IsDead)
        {
            Rigidbody.AddForce(Vector3.down * GroundingForce, ForceMode.Acceleration); // Przyciąganie do ziemi
        }

        if (!IsDead && RichAI.canMove)
        {
            ApplySeparation(); // Odpychanie od innych zombie
        }
    }

    public virtual void MoveAnimation()
    {
        return;
    }

    public virtual void TakeDamage(float amount)
    {
        Health -= amount;

        if (!IsDead && Health <= 0)
        {
            IsDead = true;
            Die();
        }
    }

    public virtual void GetStunned(float stunDuration, GameObject stunEffect)
    {
        RichAI.canMove = false;
        Animator.enabled = false;
        GameObject _stunEffect = Instantiate(stunEffect, transform.position + new Vector3(0, 1, 0), Quaternion.identity, transform);
        Destroy(_stunEffect, stunDuration);

        if (stunnedCoroutine != null)
        {
            StopCoroutine(stunnedCoroutine);
        }

        stunnedCoroutine = StartCoroutine(StunProcess(stunDuration, stunEffect));
    }

    public virtual IEnumerator StunProcess(float stunDuration, GameObject stunEffect)
    {
        yield return new WaitForSeconds(stunDuration);
        RichAI.canMove = true;
        Animator.enabled = true;
    }

    public virtual void Attack()
    {
        int _index = Random.Range(0, 2);
        Animator.SetInteger("AttackVariant", _index);

        if (attackTarget != null)
        {
            if (attackTarget.TryGetComponent(out BaseBarricade barricade))
            {
                barricade.TakeDamage(Damage);
                BaseBarricadePart part = attackTarget.GetComponent<BaseBarricadePart>();
                if (part != null) part.IsDamaged();
            }
            else if (attackTarget.CompareTag("Truck"))
            {
                ResourcesManager.Instance.TakeDamage(Damage);
            }
        }
    }

    public virtual void Die()
    {
        Instantiate(DieGibbletsFX, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        Instantiate(DieBloodExplosionFX, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        EventManager.OnEnemyDied(gameObject);
        Destroy(gameObject);
    }

    public virtual void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10 || other.CompareTag("Truck"))
        {
            attackTarget = other.gameObject;
            UpdateTarget(attackTarget.transform);
            Animator.SetBool("IsAttacking", true);
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10 || other.CompareTag("Truck"))
        {
            attackTarget = null;
            Animator.SetBool("IsAttacking", false);
            UpdateToNearestDestinationTarget();
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
        {
            IsTouchingGround = true;
        }
    }

    public virtual void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
        {
            IsTouchingGround = false;
        }
    }

    private void UpdateTarget(Transform newTarget)
    {
        if (newTarget != null && AIDestinationSetter != null)
        {
            AIDestinationSetter.target = newTarget;
            RichAI.canMove = true;
        }
    }

    private void UpdateToNearestDestinationTarget()
    {
        GameObject[] destinationTargets = GameObject.FindGameObjectsWithTag("DestinationTarget");
        if (destinationTargets.Length == 0)
        {
            Debug.LogError("Brak DestinationTargets w scenie!");
            return;
        }

        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject target in destinationTargets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = target.transform;
            }
        }

        if (nearestTarget != null)
        {
            UpdateTarget(nearestTarget);
        }
    }

    private void ApplySeparation()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Enemy")); // Warstwa zombie (np. 9)
        foreach (Collider enemy in nearbyEnemies)
        {
            if (enemy.gameObject != gameObject) // Ignoruj samego siebie
            {
                Vector3 direction = transform.position - enemy.transform.position;
                float distance = direction.magnitude;
                if (distance > 0 && distance < 1f) // Odpychaj tylko w bliskim zasięgu
                {
                    Vector3 force = direction.normalized * (SeparationForce / distance);
                    Rigidbody.AddForce(force, ForceMode.Acceleration);
                }
            }
        }
    }
}
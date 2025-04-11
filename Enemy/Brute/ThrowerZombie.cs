using System.Collections;
using UnityEngine;

public class ThrowerZombie : Enemy
{
    public GameObject Rock;
    public GameObject RockAnimationObject;
    public bool IsRockThrown = false;
    public float ThrowRange = 100f;
    public float MovementDelayValue = 3f;
    public float ThrowDelayValue = 20f;
    public float NoThrowingRange = 20f;
    public Transform Player;

    public Transform RightHand;
    private Quaternion rockRotation;
    private Vector3 rockOffset;

    public GameObject SpawnAudioCue;

    protected override void Start()
    {
        base.Start(); // Wywo³uje Start z Enemy, ustawiaj¹c najbli¿szy DestinationTarget
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        rockRotation = Quaternion.Euler(-197.048f, -112.416f, 216.638f);
        rockOffset = RightHand.position - RockAnimationObject.transform.position;

        float audioDelay = Random.Range(0f, 4f);
        Invoke("PlayAudioCue", audioDelay); // Losowy delay 0-4 sekundy
    }

    protected override void OnEnable()
    {
        base.OnEnable(); // Wywo³uje OnEnable z Enemy, ustawiaj¹c najbli¿szy DestinationTarget
        RockAnimationObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsRockThrown && attackTarget == null && Vector3.Distance(transform.position, AIDestinationSetter.target.position) <= ThrowRange)
        {
            IsRockThrown = true;
            ThrowRockAnimation();
        }

        if (RockAnimationObject.activeSelf)
        {
            RockAnimationObject.transform.position = RightHand.position + rockOffset;
            RockAnimationObject.transform.rotation = rockRotation;
        }
    }

    private void LateUpdate()
    {
        if (RockAnimationObject.activeSelf)
        {
            RockAnimationObject.transform.position = RightHand.position + rockOffset;
            RockAnimationObject.transform.rotation = rockRotation;
        }
    }

    public void ThrowRockAnimation()
    {
        StartCoroutine(ThrowingMovementDelay());
        StartCoroutine(ThrowDelay());

        Animator.SetBool("IsThrowing", true);
        RockAnimationObject.SetActive(true);
    }

    public IEnumerator ThrowingMovementDelay()
    {
        RichAI.canMove = false;
        yield return new WaitForSeconds(MovementDelayValue / 3);
        yield return new WaitForSeconds(MovementDelayValue / 3);
        ThrowRock();
        RockAnimationObject.SetActive(false);
        yield return new WaitForSeconds(MovementDelayValue / 3);
        RichAI.canMove = true;
        Animator.SetBool("IsThrowing", false);
    }

    public IEnumerator ThrowDelay()
    {
        yield return new WaitForSeconds(ThrowDelayValue);

        float distanceToTarget = Vector3.Distance(transform.position, AIDestinationSetter.target.position);
        if (distanceToTarget <= ThrowRange && distanceToTarget >= NoThrowingRange)
        {
            IsRockThrown = false;
        }
    }

    void ThrowRock()
    {
        GameObject _rock = Instantiate(Rock, RockAnimationObject.transform.position, RockAnimationObject.transform.rotation);
        Rigidbody rb = _rock.GetComponent<Rigidbody>();

        Vector3 direction = Player.position - RockAnimationObject.transform.position;

        float horizontalDistance = new Vector2(direction.x, direction.z).magnitude;
        float verticalDistance = direction.y;

        float angle = 50f * Mathf.Deg2Rad;  // launch angle in radians

        // Calculate the initial velocity
        float velocity = Mathf.Sqrt((horizontalDistance * Physics.gravity.magnitude) / Mathf.Sin(2 * angle));

        // Decompose the velocity into xz and y components
        Vector3 velocityVector = direction;
        velocityVector.y = 0;
        velocityVector = velocityVector.normalized * velocity * Mathf.Cos(angle);
        velocityVector.y = velocity * Mathf.Sin(angle);

        // Adjust the velocity to account for elevation difference
        float time = horizontalDistance / (velocity * Mathf.Cos(angle));
        velocityVector.y += (verticalDistance / time);

        rb.linearVelocity = velocityVector;
    }

    void PlayAudioCue()
    {
        if (SpawnAudioCue != null)
        {
            GameObject audio = Instantiate(SpawnAudioCue, transform.position, Quaternion.identity);
            Destroy(audio, 5f);
        }
    }

    // Nadpisz OnTriggerStay, aby Thrower nie atakowa³ wrêcz, tylko rzuca³
    public override void OnTriggerStay(Collider other)
    {
        // Thrower nie zatrzymuje siê przy barykadach ani ciê¿arówce – rzuca z dystansu
        return;
    }

    public override void OnTriggerExit(Collider other)
    {
        // Thrower nie zmienia celu po opuszczeniu triggera – kontynuuje rzucanie
        return;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : Ability
{
    public Transform ThrowPosition;
    public GameObject ObjectToThrow;
    public float ThrowForceMultiplier = 10f;

    public virtual void Throw()
    {
        StartCoroutine(AbilityDelay());
        ResourcesManager.Instance.Ammunition -= ResourceCost;

        GameObject _projectile = Instantiate(ObjectToThrow, ThrowPosition.position, Quaternion.identity);
        Rigidbody rb = _projectile.GetComponent<Rigidbody>();

        Vector3 direction = mousePosition.position - ThrowPosition.position;

        float horizontalDistance = new Vector2(direction.x, direction.z).magnitude;
        float verticalDistance = direction.y;
        
        float angle = 45f * Mathf.Deg2Rad;  // launch angle in radians

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
}

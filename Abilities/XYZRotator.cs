using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XYZRotator : MonoBehaviour
{
    public bool IsRotating = true;
    public bool IsRandomSpeed = false;
    public bool StopAfterColliding = true;
    [Header("Picked Values")]
    public float XSpeed = 1f;
    public float YSpeed = 0f;
    public float ZSpeed = 0f;
    [Header("Random Values")]
    [Range(0f, 20f)] public float RandomSpeedRange = 10f;

    private void Start()
    {
        if (IsRandomSpeed == true)
        {
            XSpeed = Random.Range(0f, RandomSpeedRange);
            YSpeed = Random.Range(0f, RandomSpeedRange);
            ZSpeed = Random.Range(0f, RandomSpeedRange);
        }
    }

    void Update()
    {
        if (IsRotating == true)
        {
            transform.Rotate(new Vector3(XSpeed, YSpeed, ZSpeed));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        IsRotating = false;
    }
}

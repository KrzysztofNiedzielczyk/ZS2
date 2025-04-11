using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float Time = 10f;

    private void Start()
    {
        Destroy(gameObject, Time);
    }
}

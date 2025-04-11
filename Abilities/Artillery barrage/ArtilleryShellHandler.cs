using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ArtilleryShellHandler : ImpactExplosionProjectile
{

    public float Speed;
    private int layerMask;

    private void Start()
    {
        layerMask |= (1 << 0);
        layerMask |= (1 << 6);
        layerMask |= (1 << 7);

        RaycastHit _hit;

        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 1000, layerMask))
        {
            SpawnedIndicator = Instantiate(Indicator, _hit.point, Quaternion.Euler(90, 0, 0));
        }
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.down * Speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Destroy(gameObject, 15f);
    }
}

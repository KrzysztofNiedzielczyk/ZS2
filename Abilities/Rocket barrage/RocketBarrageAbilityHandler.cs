using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RocketBarrageAbilityHandler : Ability
{
    private bool fireing = false;
    public Transform Muzzle;
    public GameObject Indicator;
    public GameObject Rocket;
    public int ShotsAmount = 8;
    public float ShotYRadius = 10;
    public float ShotZRadius = 5;
    public float XSpread = 5;
    public float ZSpread = 5;
    private int layerMask;

    private void Start()
    {
        layerMask |= (1 << 0);
        layerMask |= (1 << 6);
        layerMask |= (1 << 7);
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

        for (int i = 0; i < ShotsAmount; i++)
        {
            //TODO play sound

            float _yValue = Random.Range(-ShotYRadius, ShotYRadius);
            float _zValue = Random.Range(-ShotZRadius, ShotZRadius);
            GameObject _rocket = Instantiate(Rocket, Muzzle.position, Quaternion.Euler(Muzzle.eulerAngles.x, Muzzle.eulerAngles.y + _yValue, Muzzle.eulerAngles.z + _zValue));

            //TODO mouse position + lower to the ground
            float _xRandomSpread = Random.Range(-XSpread, XSpread);
            float _yRandomSpread = Random.Range(-ZSpread, ZSpread);
            Vector3 _newRocketTarget = new Vector3(mousePosition.position.x + _xRandomSpread, mousePosition.position.y + 100, mousePosition.position.z + _yRandomSpread);

            RaycastHit _hit;
            if (Physics.Raycast(_newRocketTarget, Vector3.down, out _hit, 500, layerMask))
            {
                _rocket.GetComponent<RocketBarrageRocketHandler>().TargetPosition = _hit.point;
                GameObject _spawnedIndicator = Instantiate(Indicator, _hit.point, Quaternion.Euler(90, 0, 0));
                _rocket.GetComponent<RocketBarrageRocketHandler>().SpawnedIndicator = _spawnedIndicator;

                if ( _spawnedIndicator != null )
                {
                    Destroy(_spawnedIndicator, 7);
                }
            }

            Destroy(_rocket, 7);
        }
    }
}

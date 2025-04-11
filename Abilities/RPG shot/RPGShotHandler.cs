using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RPGShotHandler : Ability
{
    public GameObject RPGRocket;
    public Transform Muzzle;
    private bool fireing = false;

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
            RocketLaunch();

            //send event to artillery timer to reset
            //EventManager.OnRPGCalled();
        }
    }

    public void RocketLaunch()
    {
        StartCoroutine(AbilityDelay());
        ResourcesManager.Instance.Ammunition -= ResourceCost;

        GameObject _rocket = Instantiate(RPGRocket, Muzzle.position, Muzzle.rotation);
        Destroy(_rocket, 10f);
    }
}

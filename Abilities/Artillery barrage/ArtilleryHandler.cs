using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArtilleryHandler : Ability
{
    public float ArtyDelayMin;
    public float ArtyDelayMax;
    public float Radius;
    public int StrikesNumber;
    public int ShellsNumber;
    public float DelayBetweenStrikes = 2f;

    private bool fireing = false;
    private bool isAimingActivated = false;

    public GameObject ArtilleryShell;
    public Transform ArtilleryZoneIndicator;

    private void Update()
    {
        //arty zone follow mouse position
        //ArtilleryZoneIndicator.position = MousePosition.position;

        /*Fireing();
        ActivateAimingMode();*/
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

    public override void OnAbilityActivation(InputAction.CallbackContext context)
    {
        isAimingActivated = !isAimingActivated;
        StartCoroutine(CallArtilleryStrike());
    }

    /*void Fireing()
    {
        //if input is stopped then do not execute code
        if (IsAvailable && isAimingActivated && fireing)
        {
            StartCoroutine(CallArtilleryStrike());
        }
    }*/

    /*void ActivateAimingMode()
    {
        //activate aim artillery mode
        if (Delay <= 0 && isAimingActivated)
        {
            ArtilleryZoneIndicator.gameObject.SetActive(true);
            isAimingActivated = true;
        }
        else
        {
            ArtilleryZoneIndicator.gameObject.SetActive(false);
            isAimingActivated = false;
        }
    }*/

    IEnumerator CallArtilleryStrike()
    {
        //isAimingActivated = false;

        StartCoroutine(AbilityDelay());
        ResourcesManager.Instance.Ammunition -= ResourceCost;

        //send event to artillery timer to reset
        //EventManager.OnArtilleryCalled();

        Vector3 position = mousePosition.position;

        for (int j = 0; j < StrikesNumber; j++)
        {
            for (int i = 0; i < ShellsNumber; ++i)
            {
                GameObject artilleryShell = Instantiate(ArtilleryShell, position + new Vector3((Random.insideUnitSphere * Radius).x, position.y + Random.Range(ArtyDelayMin, ArtyDelayMax), (Random.insideUnitSphere * Radius).z), transform.rotation);
                Destroy(artilleryShell, 10f);
            }
            yield return new WaitForSeconds(DelayBetweenStrikes);
        }
    }
}

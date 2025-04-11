using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShotgunBlastHandler : Ability
{
    private bool fireing = false;
    public Transform Muzzle;
    public GameObject ShotgunShell;
    public int ShotsAmount = 24;
    public float BlastXRadius = 10;
    public float BlastZRadius = 30;
    public List<AudioSource> ShotSFX = new List<AudioSource>();


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

        int _randomIndex = Random.Range(0, ShotSFX.Count - 1);
        ShotSFX[_randomIndex].Play();

        for (int i = 0; i < ShotsAmount/4; i++)
        {
            float _xValue = Random.Range(-BlastXRadius/2, BlastXRadius/2);
            float _zValue = Random.Range(-BlastZRadius, BlastZRadius);
            GameObject _shell = Instantiate(ShotgunShell, Muzzle.position, Quaternion.Euler(Muzzle.eulerAngles.x + _xValue, Muzzle.eulerAngles.y, Muzzle.eulerAngles.z + _zValue));

            Destroy(_shell, 5f);
        }
        for(int i = 0; i < ShotsAmount/4; i++)
        {
            float _xValue = Random.Range(-BlastXRadius, BlastXRadius);
            float _zValue = Random.Range(-BlastZRadius/2, BlastZRadius/2);
            GameObject _shell = Instantiate(ShotgunShell, Muzzle.position, Quaternion.Euler(Muzzle.eulerAngles.x + _xValue, Muzzle.eulerAngles.y, Muzzle.eulerAngles.z + _zValue));

            Destroy(_shell, 5f);
        }
        for(int i = 0; i < ShotsAmount/4; i++)
        {
            float _xValue = Random.Range(-BlastXRadius/4, BlastXRadius/4);
            float _zValue = Random.Range(-BlastZRadius/4, BlastZRadius/4);
            GameObject _shell = Instantiate(ShotgunShell, Muzzle.position, Quaternion.Euler(Muzzle.eulerAngles.x + _xValue, Muzzle.eulerAngles.y, Muzzle.eulerAngles.z + _zValue));

            Destroy(_shell, 5f);
        }
        for(int i = 0; i < ShotsAmount/4; i++)
        {
            float _xValue = Random.Range(-BlastXRadius, BlastXRadius);
            float _zValue = Random.Range(-BlastZRadius, BlastZRadius);
            GameObject _shell = Instantiate(ShotgunShell, Muzzle.position, Quaternion.Euler(Muzzle.eulerAngles.x + _xValue, Muzzle.eulerAngles.y, Muzzle.eulerAngles.z + _zValue));

            Destroy(_shell, 5f);
        }
    }
}

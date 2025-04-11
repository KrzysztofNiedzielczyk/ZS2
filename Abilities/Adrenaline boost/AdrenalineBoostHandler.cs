using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdrenalineBoostHandler : Ability
{
    [Range(0, 1)] public float TimeModifier = 0.75f;
    public float Duration = 7.5f;
    private Weapon weapon;
    public AudioSource AdrenalineStartsSFX;
    public AudioSource AdrenalineEndsSFX;

    private bool fireing = false;

    private void Start()
    {
        weapon = GetComponent<Weapon>();
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
            StartCoroutine(AdrenalineBoost());

            //send event to artillery timer to reset
            //EventManager.OnFragGrenadeCalled();
        }
    }

    public IEnumerator AdrenalineBoost()
    {
        StartCoroutine(AbilityDelay());
        ResourcesManager.Instance.Ammunition -= ResourceCost;

        AdrenalineStartsSFX.Play();

        Time.timeScale = TimeModifier;
        weapon.RotationSpeed *= (1 - TimeModifier) * 100;

        StartCoroutine(EndingAudioCounter());

        yield return new WaitForSecondsRealtime(Duration);

        Time.timeScale = 1;
        weapon.RotationSpeed /= (1 - TimeModifier) * 100;
    }

    public IEnumerator EndingAudioCounter()
    {
        yield return new WaitForSecondsRealtime(Duration - 2);
        AdrenalineEndsSFX.Play();
    }
}

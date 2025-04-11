using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashbangAbilityHandler : Throwable
{
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
            Throw();

            //send event to artillery timer to reset
            //EventManager.OnFlashbangCalled();
        }
    }
}

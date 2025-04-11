using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameActionsMenu : MonoBehaviour
{
    public GameObject ActionsMenu;

    public void OnActionsMenu(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            FlipActionsMenu();
        }
    }

    public void FlipActionsMenu()
    {
        if (TimeManager.Instance.IsGamePaused == true)
        {
            if (ActionsMenu.activeInHierarchy)
            {
                TimeManager.Instance.PauseFlip();
                ActionsMenu.SetActive(false);
            }
        }
        else
        {
            if (ActionsMenu.activeInHierarchy == false)
            {
                TimeManager.Instance.PauseFlip();
                ActionsMenu.SetActive(true);
            }
        }
    }
}

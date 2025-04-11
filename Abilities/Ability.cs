using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Ability : MonoBehaviour
{
    public float DelayValue = 5f;
    public float Delay = 0f;
    public bool IsAvailable = true;
    public float ResourceCost = 1;

    protected Transform mousePosition; // Pole do przechowywania MousePosition

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subskrypcja na wczytanie sceny
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Odsubskrypcja
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main Scene") // Tylko w scenach walki
        {
            FindMousePosition(); // Szukaj MousePosition po wczytaniu
        }
    }

    protected virtual void FindMousePosition()
    {
        GameObject mouseObj = GameObject.FindGameObjectWithTag("MousePosition");
        if (mouseObj != null)
        {
            mousePosition = mouseObj.transform;
        }
        else
        {
            Debug.LogError("MousePosition not found in scene!");
        }
    }

    public virtual void OnAbilityActivation(InputAction.CallbackContext context)
    {

    }

    public virtual IEnumerator AbilityDelay()
    {
        IsAvailable = false;
        Delay = DelayValue / ResourcesManager.Instance.AmmunitionCurrentBonus;

        while (Delay > 0)
        {
            yield return new WaitForSeconds(1);
            Delay--;
        }

        IsAvailable = true;
    }
}
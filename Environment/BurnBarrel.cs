using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnBarrel : MonoBehaviour
{
    public bool IsReady = true;
    public bool IsBaseBarrel;
    public Light Light;
    public ParticleSystem ParticleSystem;

    private void Start()
    {
        IsReady = true;
    }

    private void OnEnable()
    {
        EventManager.DayNightCycleToggled += OnDayNightCycleToggled;
    }

    private void OnDisable()
    {
        EventManager.DayNightCycleToggled -= OnDayNightCycleToggled;
    }

    public virtual void OnDayNightCycleToggled()
    {
        if (DayNightCycleHandler.Instance.IsDay())
        {
            IsReady = false;
        }
        else if (IsBaseBarrel)
        {
            IsReady = true;
        }

        if (IsReady)
        {
            Light.enabled = true;
            ParticleSystem.Play();
        }
        else
        {
            Light.enabled = false;
            ParticleSystem.Stop();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightHandler : MonoBehaviour
{
    public Light Light;

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
        Light.enabled = !Light.isActiveAndEnabled;
    }
}

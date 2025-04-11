using UnityEngine;

public class DayNightTargetHandler : MonoBehaviour
{
    public float CircleRadius = 160f; // Promień okręgu ruchu słońca

    void LateUpdate()
    {
        float cycleProgress = GameTimeManager.Instance.GetCycleProgress();

        // Pozycja SunTarget na okręgu w płaszczyźnie X-Y (wschód-zachód)
        float sunriseStart = 5f / 24f;  // 05:00 (0.2083)
        float sunsetEnd = 21f / 24f;    // 21:00 (0.875)
        float sunCycleDuration = sunsetEnd - sunriseStart; // Czas aktywności słońca (5:00-21:00)
        float sunProgress = Mathf.InverseLerp(sunriseStart, sunsetEnd, cycleProgress); // 0 (5:00) → 1 (21:00)

        if (cycleProgress >= sunriseStart && cycleProgress <= sunsetEnd)
        {
            float angle = sunProgress * Mathf.PI; // Pół okręgu (0-π) od wschodu do zachodu
            float x = Mathf.Cos(angle) * CircleRadius; // Wschód-Zachód (X), od +R do -R
            float y = Mathf.Sin(angle) * CircleRadius; // Góra-Dół (Y), od 0 do +R i z powrotem
            transform.localPosition = new Vector3(x, y, 0);
        }
        else
        {
            transform.localPosition = new Vector3(CircleRadius, 0, 0); // Poza widocznością w nocy
        }

        Debug.Log($"SunTarget Position: {transform.localPosition}, CycleProgress: {cycleProgress}, SunProgress: {sunProgress}");
    }
}
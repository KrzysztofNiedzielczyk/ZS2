using UnityEngine;
using System.Collections;

public class DayNightCycleHandler : MonoBehaviour
{
    private static DayNightCycleHandler _instance;
    public static DayNightCycleHandler Instance => _instance;

    private Light directionalLight;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    void Start()
    {
        directionalLight = GetComponent<Light>();
        if (directionalLight == null)
        {
            Debug.LogError("Brak komponentu Light na Directional Light!");
        }
    }

    void Update()
    {
        float cycleProgress = GameTimeManager.Instance.GetCycleProgress();

        // Ruch słońca (łuk nad sceną z ruchem od prawej do lewej)
        float sunriseStart = 5f / 24f;  // 05:00 (0.2083)
        float midday = 12.5f / 24f;     // 12:30 (0.5208)
        float sunsetEnd = 21f / 24f;    // 21:00 (0.875)
        float sunCycleDuration = sunsetEnd - sunriseStart; // 5:00-21:00
        float sunProgress;

        if (cycleProgress >= sunriseStart && cycleProgress <= sunsetEnd)
        {
            sunProgress = Mathf.InverseLerp(sunriseStart, sunsetEnd, cycleProgress); // 0 (5:00) → 1 (21:00)
            float angleX;
            if (sunProgress <= 0.5f) // 5:00-12:30
            {
                angleX = Mathf.Lerp(0f, 60f, sunProgress / 0.5f); // 0° → 60°
            }
            else // 12:30-21:00
            {
                angleX = Mathf.Lerp(60f, 0f, (sunProgress - 0.5f) / 0.5f); // 60° → 0°
            }
            float angleY = Mathf.Lerp(-90f, 90f, sunProgress); // Wschód (-90°, prawo) → Zachód (90°, lewo)
            transform.localEulerAngles = new Vector3(angleX, angleY, 0); // X dla łuku, Y dla wschód-zachód
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, -90f, 0); // Poza widocznością w nocy (wschód)
        }

        // Intensywność i temperatura barwowa
        float dayStart = 6f / 24f;      // 06:00 (0.25)
        float dayEnd = 20f / 24f;       // 20:00 (0.8333)

        float intensity;
        float temp;

        if (cycleProgress < sunriseStart || cycleProgress >= sunsetEnd)
        {
            intensity = 0f; // Noc: 21:00-5:00
            temp = 2000f;
        }
        else if (cycleProgress < dayStart)
        {
            // Wschód: 5:00-6:00
            float sunriseProgress = Mathf.InverseLerp(sunriseStart, dayStart, cycleProgress);
            intensity = Mathf.Lerp(0f, 0.2f, sunriseProgress); // 0 → 0.2
            temp = Mathf.Lerp(2000f, 2500f, sunriseProgress);   // 2000K → 2500K
        }
        else if (cycleProgress < midday)
        {
            // Dzień rośnie: 6:00-12:30
            float morningProgress = Mathf.InverseLerp(dayStart, midday, cycleProgress);
            intensity = Mathf.Lerp(0.2f, 1.5f, morningProgress); // 0.2 → 1.5
            temp = Mathf.Lerp(2500f, 6500f, morningProgress);    // 2500K → 6500K
        }
        else if (cycleProgress < dayEnd)
        {
            // Dzień spada: 12:30-20:00
            float afternoonProgress = Mathf.InverseLerp(midday, dayEnd, cycleProgress);
            intensity = Mathf.Lerp(1.5f, 0.2f, afternoonProgress); // 1.5 → 0.2
            temp = Mathf.Lerp(6500f, 2500f, afternoonProgress);    // 6500K → 2500K
        }
        else
        {
            // Zmierzch: 20:00-21:00
            float sunsetProgress = Mathf.InverseLerp(dayEnd, sunsetEnd, cycleProgress);
            intensity = Mathf.Lerp(0.2f, 0f, sunsetProgress); // 0.2 → 0
            temp = Mathf.Lerp(2500f, 2000f, sunsetProgress);  // 2500K → 2000K
        }

        directionalLight.intensity = intensity;
        directionalLight.colorTemperature = temp;
    }

    public bool IsDay()
    {
        float cycleProgress = GameTimeManager.Instance.GetCycleProgress();
        return cycleProgress >= 6f / 24f && cycleProgress < 18f / 24f; // Dzień: 6:00-18:00
    }

    public IEnumerator SmoothAdvanceTime(float gameHours, float realDuration = 1f)
    {
        float gameSecondsToAdd = gameHours * 3600f;
        float startTime = GameTimeManager.Instance.GetTotalGameTimeSeconds();
        float targetTime = startTime + gameSecondsToAdd;
        float elapsed = 0f;

        Debug.Log($"Advancing time: {gameHours} game hours = {gameSecondsToAdd} game seconds, real duration: {realDuration}s");

        while (elapsed < realDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / realDuration;
            float newTime = Mathf.Lerp(startTime, targetTime, t);
            GameTimeManager.Instance.SetTotalGameTimeSeconds(newTime);
            yield return null;
        }
        GameTimeManager.Instance.SetTotalGameTimeSeconds(targetTime);
    }
}
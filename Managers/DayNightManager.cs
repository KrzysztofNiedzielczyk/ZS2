using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    private static DayNightManager _instance;
    public static DayNightManager Instance => _instance ??= FindFirstObjectByType<DayNightManager>() ?? new GameObject("Directional Light").AddComponent<DayNightManager>();

    public DayNightCycleHandler dayNightCycleHandler; // Referencja do skryptu na tym samym obiekcie

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // Przenosi Directional Light z DayNightCycleHandler
        Debug.Log("DayNightManager Awake – œwiat³o gotowe!");

        // Sprawdzenie DayNightCycleHandler na tym obiekcie
        dayNightCycleHandler = GetComponent<DayNightCycleHandler>();
        if (dayNightCycleHandler == null)
        {
            Debug.LogError("DayNightCycleHandler nie znaleziony na Directional Light!");
        }
    }
}
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    private static GameTimeManager _instance;
    public static GameTimeManager Instance => _instance ??= FindFirstObjectByType<GameTimeManager>() ?? new GameObject("GameTimeManager").AddComponent<GameTimeManager>();

    [SerializeField] public float gameTimePerRealSecond = 60f; // Szybko�� up�ywu czasu w grze (sekundy gry na sekund� rzeczywist�)
    [SerializeField] private float cycleLength = 86400f; // D�ugo�� cyklu dnia (24h = 86400 sekund gry)
    private float totalGameTimeSeconds = 21600f; // Start o 6:00 rano (6 godzin * 3600 sekund)

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        totalGameTimeSeconds += Time.deltaTime * gameTimePerRealSecond;
    }

    public float GetTotalGameTimeSeconds()
    {
        return totalGameTimeSeconds;
    }

    public void AdvanceTime(float gameHours)
    {
        totalGameTimeSeconds += gameHours * 3600f;
    }

    public void SetTotalGameTimeSeconds(float newTime)
    {
        totalGameTimeSeconds = newTime;
    }

    // Nowa metoda: zwraca post�p cyklu (0-1)
    public float GetCycleProgress()
    {
        return (totalGameTimeSeconds % cycleLength) / cycleLength;
    }

    // Nowa metoda: zwraca d�ugo�� cyklu (dla synchronizacji)
    public float GetCycleLength()
    {
        return cycleLength;
    }
}
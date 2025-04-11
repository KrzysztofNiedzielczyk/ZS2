using UnityEngine;

public class TruckManager : MonoBehaviour
{
    private static TruckManager _instance;
    public static TruckManager Instance => _instance ??= FindFirstObjectByType<TruckManager>() ?? new GameObject("Truck").AddComponent<TruckManager>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("TruckManager Awake – ciê¿arówka gotowa!");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("StaticZombie"))
        {
            ResourcesManager.Instance.Health -= 50;
            Debug.Log("Kolizja z przeszkod¹ lub zombie – zdrowie: " + ResourcesManager.Instance.Health);
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

public class TravelManager : MonoBehaviour
{
    private static TravelManager _instance;
    public static TravelManager Instance => _instance ??= FindFirstObjectByType<TravelManager>() ?? new GameObject("TravelManager").AddComponent<TravelManager>();

    [SerializeField] private TerrainTypeSO[] allTerrainTypes;
    private List<TerrainCell> visitedLocations = new List<TerrainCell>();
    private int currentPosition = 0;
    private TerrainCell currentLocation;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // TravelManager trwa³y miêdzy scenami
    }

    void Start()
    {
        TerrainTypeSO roadTerrain = System.Array.Find(allTerrainTypes, t => t.terrainName == "Road");
        if (roadTerrain == null) roadTerrain = allTerrainTypes[0];
        visitedLocations.Add(new TerrainCell { terrainType = roadTerrain });
    }

    public TerrainCell GetCurrentLocation()
    {
        if (currentPosition >= visitedLocations.Count)
        {
            TerrainTypeSO terrainType = allTerrainTypes[Random.Range(0, allTerrainTypes.Length)];
            visitedLocations.Add(new TerrainCell { terrainType = terrainType });
        }
        return visitedLocations[currentPosition];
    }

    public void MoveForward(int choiceIndex)
    {
        currentPosition++;
    }
}
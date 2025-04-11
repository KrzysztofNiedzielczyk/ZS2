using UnityEngine;

[CreateAssetMenu(fileName = "TerrainType", menuName = "GameEvents/TerrainType")]
public class TerrainTypeSO : ScriptableObject
{
    public string terrainName;          // "Forest", "City", itd.
    public Sprite icon;                 // Ikona terenu
    public int minEnemies;              // Minimalna liczba wrogów
    public int maxEnemies;              // Maksymalna liczba wrogów
    public float contaminationLevel;    // Poziom ska¿enia (0-1)
    [TextArea] public string description; // Opis terenu (opcjonalny)
}
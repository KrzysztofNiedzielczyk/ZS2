using UnityEngine;

[System.Serializable]
public class GameEventEffect
{
    public string effectDescription;
    public float chance;
    public int fuelChange;
    public int ammoChange;
    public int scrapChange;
    public int survivorsChange;
    public int healthChange;
    public int enemySpawnCount;
    public int delayDays;
    public int durationDays;
    public Sprite effectIcon;
    public bool triggerCombatScene;
    public string combatSceneName = "DefaultCombat"; // Domyœlna scena, jeœli brak specyficznej
}

[System.Serializable]
public class GameEventOption
{
    public string optionDescription;
    public GameEventEffect[] possibleEffects;
    public float timeCostHours; // Nowe pole: czas w godzinach (np. 1.0 dla 1 godziny)
}

[CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/GameEvent")]
public class GameEvent : ScriptableObject
{
    public string eventTitle;
    [TextArea] public string eventDescription;
    public Sprite eventIcon;
    public GameEventOption[] options;
}
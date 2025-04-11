using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameEventDatabase", menuName = "GameEvents/GameEventDatabase")]
public class GameEventDatabase : ScriptableObject
{
    public List<GameEvent> forestEvents;
    public List<GameEvent> cityEvents;
    public List<GameEvent> mountainEvents;
    public List<GameEvent> desertEvents;
    public List<GameEvent> coastEvents;
    public List<GameEvent> roadEvents;
    public List<GameEvent> villageEvents;
    public List<GameEvent> suburbanEvents;
    public List<GameEvent> industrialEvents;
    public List<GameEvent> highwayEvents;

    public GameEvent GetRandomEvent(string terrainName)
    {
        List<GameEvent> eventList = GetEventList(terrainName);
        return eventList[Random.Range(0, eventList.Count)];
    }

    private List<GameEvent> GetEventList(string terrainName)
    {
        switch (terrainName)
        {
            case "Forest": return forestEvents;
            case "City": return cityEvents;
            case "Mountain": return mountainEvents;
            case "Desert": return desertEvents;
            case "Coast": return coastEvents;
            case "Road": return roadEvents;
            case "Village": return villageEvents;
            case "Suburban": return suburbanEvents;
            case "Industrial": return industrialEvents;
            case "Highway": return highwayEvents;
            default: return roadEvents;
        }
    }
}
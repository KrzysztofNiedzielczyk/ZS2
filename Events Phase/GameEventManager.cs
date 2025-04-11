using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEventManager : MonoBehaviour
{
    private static GameEventManager _instance;
    public static GameEventManager Instance => _instance ??= FindFirstObjectByType<GameEventManager>() ?? new GameObject("GameEventManager").AddComponent<GameEventManager>();

    [SerializeField] private GameEventDatabase eventDatabase;
    private string pendingCombatSceneName;
    private int pendingEnemySpawnCount;
    private bool isCombatNext;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameEventManager Awake");
    }

    public GameEvent GetRandomEvent(string terrainName)
    {
        if (eventDatabase == null)
        {
            Debug.LogError("eventDatabase is null!");
            return null;
        }
        GameEvent ev = eventDatabase.GetRandomEvent(terrainName);
        Debug.Log($"Wylosowano wydarzenie: {ev?.eventTitle}");
        return ev;
    }

    public void ProcessEventOption(GameEvent gameEvent, int optionIndex, out GameEventEffect chosenEffect)
    {
        if (gameEvent == null || gameEvent.options == null || optionIndex < 0 || optionIndex >= gameEvent.options.Length)
        {
            Debug.LogError($"Invalid gameEvent or optionIndex: gameEvent={gameEvent}, optionIndex={optionIndex}");
            chosenEffect = null;
            return;
        }

        GameEventOption chosenOption = gameEvent.options[optionIndex];
        chosenEffect = ChooseEffect(chosenOption);
        ApplyEffect(chosenEffect);
    }

    private GameEventEffect ChooseEffect(GameEventOption option)
    {
        if (option == null || option.possibleEffects == null || option.possibleEffects.Length == 0)
        {
            Debug.LogError("Chosen option or its effects are null or empty!");
            return null;
        }

        float roll = Random.value;
        float cumulativeChance = 0f;
        foreach (var effect in option.possibleEffects)
        {
            cumulativeChance += effect.chance;
            if (roll <= cumulativeChance)
            {
                return effect;
            }
        }
        return option.possibleEffects[0];
    }

    private void ApplyEffect(GameEventEffect effect)
    {
        if (effect == null)
        {
            Debug.LogError("Effect is null!");
            return;
        }

        if (effect.triggerCombatScene)
        {
            pendingCombatSceneName = effect.combatSceneName;
            pendingEnemySpawnCount = effect.enemySpawnCount;
            isCombatNext = true;
            Debug.Log($"Combat prepared: {effect.combatSceneName} with {effect.enemySpawnCount} enemies");
        }
        else
        {
            isCombatNext = false;
            if (ResourcesManager.Instance == null)
            {
                Debug.LogError("ResourcesManager.Instance is null!");
                return;
            }
            ResourcesManager.Instance.AddFuel(effect.fuelChange);
            ResourcesManager.Instance.AddAmmunition(effect.ammoChange);
            ResourcesManager.Instance.AddScrapMetal(effect.scrapChange);
            ResourcesManager.Instance.AddSurvivors(effect.survivorsChange);
            ResourcesManager.Instance.Health += effect.healthChange;
            Debug.Log($"Applied effect: {effect.effectDescription}");
        }
    }

    public void StartCombat()
    {
        if (!string.IsNullOrEmpty(pendingCombatSceneName))
        {
            Debug.Log($"StartCombat: £adowanie sceny {pendingCombatSceneName} z {pendingEnemySpawnCount} wrogami");
            SceneManager.LoadScene(pendingCombatSceneName);
            CombatData.Instance.SetCombatData(pendingEnemySpawnCount);
            pendingCombatSceneName = null;
            pendingEnemySpawnCount = 0;
        }
        else
        {
            Debug.LogError("StartCombat: Brak pendingCombatSceneName!");
        }
    }

    public bool IsCombatSceneNext()
    {
        return isCombatNext;
    }

    public void ResetCombatFlag()
    {
        isCombatNext = false;
        Debug.Log("Flaga isCombatNext zresetowana");
    }
}
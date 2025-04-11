using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TravelUI : MonoBehaviour
{
    public static TravelUI Instance { get; private set; }
    private GameEvent currentEvent;
    private GameEventEffect lastEffect;
    private string currentContext = "Start";

    [SerializeField] private float realDuration = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.introText.text = "You wake up in the truck after a long night...";
            UIManager.Instance.fightButton.gameObject.SetActive(false);
            UIManager.Instance.nextButton.gameObject.SetActive(false);
            UIManager.Instance.resultText.gameObject.SetActive(false);
            UIManager.Instance.sleepButton.gameObject.SetActive(true);
            UIManager.Instance.sleepButton.onClick.RemoveAllListeners();
            UIManager.Instance.sleepButton.onClick.AddListener(OnSleepButtonClicked);
            UIManager.Instance.ShowSleepControls();
            Invoke("ShowFirstEvent", 3f);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Scene" && UIManager.Instance != null)
        {
            UIManager.Instance.ShowTravelMenu();
            UIManager.Instance.ShowSleepControls();
            if (TruckManager.Instance != null)
            {
                Player player = TruckManager.Instance.GetComponentInChildren<Player>(true);
                if (player != null) player.gameObject.SetActive(false);
            }

            if (PlayerPrefs.GetInt("FromCombat", 0) == 1)
            {
                currentContext = "Combat";
                ShowTravelOptions();
                PlayerPrefs.SetInt("FromCombat", 0);
            }
            else if (lastEffect != null && lastEffect.triggerCombatScene)
            {
                currentContext = "Combat";
                UIManager.Instance.eventTitleText.text = "";
                UIManager.Instance.eventDescText.text = "";
                foreach (var button in UIManager.Instance.choiceButtons)
                {
                    button.gameObject.SetActive(false);
                }
                string resultDetails = GetResultDetails(lastEffect);
                UIManager.Instance.resultText.text = $"{lastEffect.effectDescription}\n{resultDetails}";
                UIManager.Instance.resultText.gameObject.SetActive(true);
                UIManager.Instance.fightButton.gameObject.SetActive(false);
                UIManager.Instance.nextButton.gameObject.SetActive(true);
                UIManager.Instance.nextButton.onClick.RemoveAllListeners();
                UIManager.Instance.nextButton.onClick.AddListener(() =>
                {
                    UIManager.Instance.resultText.gameObject.SetActive(false);
                    ShowTravelOptions();
                });
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void ShowFirstEvent()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.introText.gameObject.SetActive(false);
            TerrainCell currentLocation = TravelManager.Instance.GetCurrentLocation();
            currentEvent = GameEventManager.Instance.GetRandomEvent(currentLocation.terrainType.terrainName);
            if (currentEvent == null)
            {
                Debug.LogError("No event loaded for terrain: " + currentLocation.terrainType.terrainName);
                return;
            }
            currentContext = "Event";
            UIManager.Instance.resultText.gameObject.SetActive(false);
            UIManager.Instance.fightButton.gameObject.SetActive(false);
            UIManager.Instance.nextButton.gameObject.SetActive(false);
            UpdateUIWithEvent();
        }
    }

    void ShowTravelOptions()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideSleepControls();
            UIManager.Instance.ShowTravelMenu();
            UIManager.Instance.introText.gameObject.SetActive(false);

            switch (currentContext)
            {
                case "Combat":
                    UIManager.Instance.eventTitleText.text = "Combat ended...";
                    UIManager.Instance.eventDescText.text = "The last zombie falls. The truck survived the fight. What's next?";
                    break;
                case "Event":
                    UIManager.Instance.eventTitleText.text = "Event concluded...";
                    UIManager.Instance.eventDescText.text = "The situation resolves. Time to decide your next move.";
                    break;
                default:
                    UIManager.Instance.eventTitleText.text = "On the road...";
                    UIManager.Instance.eventDescText.text = "The truck is ready. What’s your next step?";
                    break;
            }

            UIManager.Instance.resultText.gameObject.SetActive(false);
            UIManager.Instance.fightButton.gameObject.SetActive(false);
            UIManager.Instance.nextButton.gameObject.SetActive(false);

            for (int i = 0; i < UIManager.Instance.choiceButtons.Length; i++)
            {
                UIManager.Instance.choiceButtons[i].gameObject.SetActive(true);
                int index = i;
                UIManager.Instance.choiceButtons[i].onClick.RemoveAllListeners();
                switch (i)
                {
                    case 0:
                        UIManager.Instance.choiceButtons[i].GetComponentInChildren<TMP_Text>().text = "Repair truck (2h, 50 Scrap)";
                        UIManager.Instance.choiceButtons[i].onClick.AddListener(() => OnRepairSelected(index));
                        break;
                    case 1:
                        UIManager.Instance.choiceButtons[i].GetComponentInChildren<TMP_Text>().text = "Rest (6h)";
                        UIManager.Instance.choiceButtons[i].onClick.AddListener(() => OnRestSelected(index));
                        break;
                    case 2:
                        UIManager.Instance.choiceButtons[i].GetComponentInChildren<TMP_Text>().text = "Drive forward (1h)";
                        UIManager.Instance.choiceButtons[i].onClick.AddListener(() => OnDriveSelected(index));
                        break;
                    default:
                        UIManager.Instance.choiceButtons[i].gameObject.SetActive(false);
                        break;
                }
            }
            Debug.Log($"Pokazano Travel Options w kontekœcie: {currentContext}");
        }
    }

    void UpdateUIWithEvent()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.eventTitleText.text = currentEvent.eventTitle;
            UIManager.Instance.eventDescText.text = currentEvent.eventDescription;

            for (int i = 0; i < UIManager.Instance.choiceButtons.Length; i++)
            {
                if (i < currentEvent.options.Length)
                {
                    UIManager.Instance.choiceButtons[i].gameObject.SetActive(true);
                    string optionText = $"{currentEvent.options[i].optionDescription} ({currentEvent.options[i].timeCostHours}h)";
                    UIManager.Instance.choiceButtons[i].GetComponentInChildren<TMP_Text>().text = optionText;
                    int index = i;
                    UIManager.Instance.choiceButtons[i].onClick.RemoveAllListeners();
                    UIManager.Instance.choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
                }
                else
                {
                    UIManager.Instance.choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void OnChoiceSelected(int choiceIndex)
    {
        if (currentEvent == null || choiceIndex < 0 || choiceIndex >= currentEvent.options.Length)
        {
            Debug.LogError("Invalid option selection or event is null!");
            return;
        }
        GameEventManager.Instance.ProcessEventOption(currentEvent, choiceIndex, out lastEffect);
        TravelManager.Instance.MoveForward(choiceIndex);

        if (UIManager.Instance != null)
        {
            foreach (var button in UIManager.Instance.choiceButtons)
            {
                button.gameObject.SetActive(false);
            }

            string resultDetails = GetResultDetails(lastEffect);
            UIManager.Instance.resultText.text = $"{lastEffect.effectDescription}\n{resultDetails}";
            UIManager.Instance.resultText.gameObject.SetActive(true);

            if (lastEffect.triggerCombatScene)
            {
                UIManager.Instance.fightButton.gameObject.SetActive(true);
                UIManager.Instance.fightButton.onClick.RemoveAllListeners();
                UIManager.Instance.fightButton.onClick.AddListener(() => StartCombat());
                UIManager.Instance.nextButton.gameObject.SetActive(false);
            }
            else
            {
                UIManager.Instance.fightButton.gameObject.SetActive(false);
                UIManager.Instance.nextButton.gameObject.SetActive(true);
                UIManager.Instance.nextButton.onClick.RemoveAllListeners();
                UIManager.Instance.nextButton.onClick.AddListener(() =>
                {
                    Debug.Log("Next Button clicked");
                    UIManager.Instance.HideTravelMenu();
                    UIManager.Instance.resultText.gameObject.SetActive(false);
                    currentContext = "Event";
                    ShowTravelOptions();
                });
                StartCoroutine(DayNightCycleHandler.Instance.SmoothAdvanceTime(currentEvent.options[choiceIndex].timeCostHours, realDuration));
            }
        }
    }

    void OnRepairSelected(int choiceIndex)
    {
        if (ResourcesManager.Instance.ScrapMetal >= 50)
        {
            ResourcesManager.Instance.SubtractScrapMetal(50);
            ResourcesManager.Instance.Health = Mathf.Min(ResourcesManager.Instance.MaxHealth, ResourcesManager.Instance.Health + 200);
            UIManager.Instance.resultText.text = "Repairing truck for 50 Scrap. Health restored by 200...";
            StartCoroutine(AdvanceTimeAndFinalize(2f, "Repaired truck for 50 Scrap. Health restored by 200."));
        }
        else
        {
            UIManager.Instance.resultText.text = "Not enough Scrap Metal to repair!";
            FinalizeChoice();
        }
    }

    void OnRestSelected(int choiceIndex)
    {
        if (ResourcesManager.Instance.TrySleep(6f))
        {
            UIManager.Instance.resultText.text = "Resting for 6 hours...";
            StartCoroutine(AdvanceTimeAndFinalize(6f, "Rested for 6 hours. Sleep Debt refreshed."));
        }
        else
        {
            UIManager.Instance.resultText.text = "It's not safe to sleep now!";
            FinalizeChoice();
        }
    }

    void OnDriveSelected(int choiceIndex)
    {
        UIManager.Instance.resultText.text = "Driving forward...";
        StartCoroutine(AdvanceTimeAndTriggerEvent(1f)); // Zamiast sceny jazdy – nowe wydarzenie
    }

    private IEnumerator AdvanceTimeAndFinalize(float gameHours, string finalMessage)
    {
        foreach (var button in UIManager.Instance.choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
        UIManager.Instance.resultText.gameObject.SetActive(true);
        UIManager.Instance.nextButton.gameObject.SetActive(false);

        yield return StartCoroutine(DayNightCycleHandler.Instance.SmoothAdvanceTime(gameHours, realDuration));

        UIManager.Instance.resultText.text = finalMessage;
        UIManager.Instance.nextButton.gameObject.SetActive(true);
        UIManager.Instance.nextButton.onClick.RemoveAllListeners();
        UIManager.Instance.nextButton.onClick.AddListener(() =>
        {
            UIManager.Instance.resultText.gameObject.SetActive(false);
            ShowTravelOptions();
        });
    }

    private IEnumerator AdvanceTimeAndTriggerEvent(float gameHours)
    {
        foreach (var button in UIManager.Instance.choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
        UIManager.Instance.resultText.gameObject.SetActive(true);
        UIManager.Instance.nextButton.gameObject.SetActive(false);

        yield return StartCoroutine(DayNightCycleHandler.Instance.SmoothAdvanceTime(gameHours, realDuration));

        TerrainCell currentLocation = TravelManager.Instance.GetCurrentLocation();
        currentEvent = GameEventManager.Instance.GetRandomEvent(currentLocation.terrainType.terrainName);
        if (currentEvent == null)
        {
            Debug.LogError("No event loaded for terrain: " + currentLocation.terrainType.terrainName);
            ShowTravelOptions();
        }
        else
        {
            currentContext = "Event";
            UIManager.Instance.resultText.gameObject.SetActive(false);
            UpdateUIWithEvent();
        }
    }

    void FinalizeChoice()
    {
        UIManager.Instance.resultText.gameObject.SetActive(true);
        UIManager.Instance.nextButton.gameObject.SetActive(true);
        UIManager.Instance.nextButton.onClick.RemoveAllListeners();
        UIManager.Instance.nextButton.onClick.AddListener(() =>
        {
            UIManager.Instance.resultText.gameObject.SetActive(false);
            ShowTravelOptions();
        });
        foreach (var button in UIManager.Instance.choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    void StartCombat()
    {
        if (lastEffect != null && lastEffect.triggerCombatScene && UIManager.Instance != null)
        {
            UIManager.Instance.HideTravelMenu();
            UIManager.Instance.HideSleepControls();
            if (TruckManager.Instance != null)
            {
                Player player = TruckManager.Instance.GetComponentInChildren<Player>(true);
                if (player != null) player.gameObject.SetActive(true);
            }
            GameEventManager.Instance.StartCombat();
        }
    }

    void OnSleepButtonClicked()
    {
        if (UIManager.Instance != null)
        {
            if (ResourcesManager.Instance.TrySleep(6f))
            {
                UIManager.Instance.sleepText.text = "You slept for 6 hours. Feeling refreshed!";
                UIManager.Instance.sleepText.gameObject.SetActive(true);
                UIManager.Instance.nextButton.gameObject.SetActive(true);
                StartCoroutine(DayNightCycleHandler.Instance.SmoothAdvanceTime(6f, realDuration));
            }
            else
            {
                UIManager.Instance.sleepText.text = "It's not safe to sleep now!";
                UIManager.Instance.sleepText.gameObject.SetActive(true);
            }
        }
    }

    string GetResultDetails(GameEventEffect effect)
    {
        string details = "";
        if (effect.fuelChange != 0) details += $"Fuel: {(effect.fuelChange > 0 ? "+" : "")}{effect.fuelChange}\n";
        if (effect.ammoChange != 0) details += $"Ammo: {(effect.ammoChange > 0 ? "+" : "")}{effect.ammoChange}\n";
        if (effect.scrapChange != 0) details += $"Scrap: {(effect.scrapChange > 0 ? "+" : "")}{effect.scrapChange}\n";
        if (effect.survivorsChange != 0) details += $"Survivors: {(effect.survivorsChange > 0 ? "+" : "")}{effect.survivorsChange}\n";
        if (effect.healthChange != 0) details += $"Health: {(effect.healthChange > 0 ? "+" : "")}{effect.healthChange}\n";
        if (effect.enemySpawnCount > 0) details += $"Zombies: {effect.enemySpawnCount}";
        return details.Trim();
    }
}
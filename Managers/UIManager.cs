using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<UIManager>();
                if (_instance == null)
                {
                    GameObject uiManagerObj = new GameObject("UIManager");
                    _instance = uiManagerObj.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }

    public GameObject HealthBar;
    public GameObject ScrapMetalBar;
    public GameObject AmmunitionBar;
    public GameObject FuelBar;
    public GameObject SleepDebtBar;
    public Slider HealthBarSlider;
    public Slider ScrapMetalBarSlider;
    public Slider AmmunitionBarSlider;
    public Slider FuelBarSlider;
    public Slider SleepDebtBarSlider;
    public TMP_Text survivorsText;
    public TMP_Text clockText;
    public Button sleepButton;
    public TMP_Text sleepText;
    public GameObject sleepControls;

    public GameObject travelMenu;
    public TMP_Text introText;
    public TMP_Text eventTitleText;
    public TMP_Text eventDescText;
    public Button[] choiceButtons;
    public TMP_Text resultText;
    public Button fightButton;
    public Button nextButton;

    public GameObject defencePanel; // Panel struktur
    public Button[] defenceButtons; // Przyciski w DefencePanel (6)

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

    void Start()
    {
        if (HealthBarSlider == null || ScrapMetalBarSlider == null || AmmunitionBarSlider == null ||
            FuelBarSlider == null || SleepDebtBarSlider == null || survivorsText == null || clockText == null)
        {
            Debug.LogError("Jedna lub wiêcej referencji w UIManager (Bars) jest null!");
        }
        else
        {
            HealthBarSlider.maxValue = ResourcesManager.Instance.MaxHealth;
            ScrapMetalBarSlider.maxValue = ResourcesManager.Instance.OverflowingLevelScrapMetal;
            AmmunitionBarSlider.maxValue = ResourcesManager.Instance.OverflowingLevelAmmunition;
            FuelBarSlider.maxValue = ResourcesManager.Instance.OverflowingLevelFuel;
            SleepDebtBarSlider.maxValue = ResourcesManager.Instance.MaxSleepDebt;
            UpdateSurvivorsText();
            UpdateClock();
        }

        if (travelMenu == null || introText == null || eventTitleText == null || eventDescText == null ||
            choiceButtons == null || resultText == null || fightButton == null || nextButton == null)
        {
            Debug.LogError("Jedna lub wiêcej referencji w UIManager (Travel Menu) jest null!");
        }

        if (sleepControls == null)
        {
            Debug.LogError("SleepControls nie przypisany w UIManager!");
        }

        if (defencePanel == null || defenceButtons == null || defenceButtons.Length != 6)
        {
            Debug.LogError("DefencePanel lub defenceButtons nie przypisane lub niew³aœciwa liczba przycisków w UIManager!");
        }
        else
        {
            defencePanel.SetActive(false);
            // Przypisanie zdarzeñ do przycisków
            for (int i = 0; i < defenceButtons.Length; i++)
            {
                int index = i; // Zachowaj indeks w closure
                defenceButtons[i].onClick.RemoveAllListeners();
                defenceButtons[i].onClick.AddListener(() => CombatManager.Instance.SelectDefence(index));
            }
        }
    }

    void Update()
    {
        if (HealthBarSlider != null) HealthBarSlider.value = ResourcesManager.Instance.Health;
        if (ScrapMetalBarSlider != null) ScrapMetalBarSlider.value = ResourcesManager.Instance.ScrapMetal;
        if (AmmunitionBarSlider != null) AmmunitionBarSlider.value = ResourcesManager.Instance.Ammunition;
        if (FuelBarSlider != null) FuelBarSlider.value = ResourcesManager.Instance.Fuel;
        if (SleepDebtBarSlider != null) SleepDebtBarSlider.value = ResourcesManager.Instance.SleepDebt;
        if (survivorsText != null) UpdateSurvivorsText();
        if (clockText != null) UpdateClock();

        // Klawisze jako dodatkowa opcja (opcjonalne)
        if (CombatManager.Instance.isPreparing)
        {
            if (Keyboard.current[Key.Digit1].wasPressedThisFrame) CombatManager.Instance.SelectDefence(0);
            if (Keyboard.current[Key.Digit2].wasPressedThisFrame) CombatManager.Instance.SelectDefence(1);
            if (Keyboard.current[Key.Digit3].wasPressedThisFrame) CombatManager.Instance.SelectDefence(2);
            if (Keyboard.current[Key.Digit4].wasPressedThisFrame) CombatManager.Instance.SelectDefence(3);
            if (Keyboard.current[Key.Digit5].wasPressedThisFrame) CombatManager.Instance.SelectDefence(4);
            if (Keyboard.current[Key.Digit6].wasPressedThisFrame) CombatManager.Instance.SelectDefence(5);
        }
    }

    private void UpdateSurvivorsText()
    {
        survivorsText.text = $"Survivors: {ResourcesManager.Instance.Survivors}/{ResourcesManager.Instance.MaxSurvivors}";
    }

    private void UpdateClock()
    {
        float totalGameTimeSeconds = GameTimeManager.Instance.GetTotalGameTimeSeconds();
        float totalGameTimeHours = totalGameTimeSeconds / 3600f;
        int days = Mathf.FloorToInt(totalGameTimeHours / 24f) + 1;
        float hoursInDay = totalGameTimeHours % 24f;
        int hours = Mathf.FloorToInt(hoursInDay);
        int minutes = Mathf.FloorToInt((hoursInDay - hours) * 60);
        string lightDark = DayNightManager.Instance.dayNightCycleHandler.IsDay() ? "Light" : "Dark";
        clockText.text = $"Day {days} {lightDark} {hours:00}:{minutes:00}";
    }

    public void ShowTravelMenu() { if (travelMenu != null) travelMenu.SetActive(true); }
    public void HideTravelMenu() { if (travelMenu != null) travelMenu.SetActive(false); }
    public void ShowSleepControls() { if (sleepControls != null) sleepControls.SetActive(true); }
    public void HideSleepControls() { if (sleepControls != null) sleepControls.SetActive(false); }
    public void ShowDefencePanel() { if (defencePanel != null) defencePanel.SetActive(true); }
    public void HideDefencePanel() { if (defencePanel != null) defencePanel.SetActive(false); }
}
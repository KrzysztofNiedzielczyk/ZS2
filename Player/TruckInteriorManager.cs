using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TruckInteriorManager : MonoBehaviour
{
    public static TruckInteriorManager Instance { get; private set; }

    public GameObject interiorUI;
    public Button checkResourcesButton; // Nowy przycisk zamiast repair/rest/drive
    public TextMeshProUGUI monologueText;
    public TextMeshProUGUI resourcesText; // Nowy tekst do wyœwietlania zasobów

    private string[] playerMonologues = { "Gotta keep moving...", "Safe Haven’s out there somewhere.", "No time to rest." };
    private string[] survivorMonologues = { "Need more fuel soon.", "Those things are relentless.", "I miss normal life." };

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Scene")
        {
            if (interiorUI != null)
            {
                interiorUI.SetActive(false); // Wy³¹cz Interior UI na starcie Main Scene
            }
        }
    }

    public void InitializeInterior(TruckPlayer truckPlayer)
    {
        truckPlayer.transform.SetParent(TruckManager.Instance.transform);
        truckPlayer.transform.localPosition = new Vector3(0, 1f, 0);
        SetupUI();
        if (interiorUI != null)
        {
            interiorUI.SetActive(true); // W³¹cz Interior UI tylko przy inicjalizacji
        }
        UpdateMonologue();
        UpdateResourcesDisplay();
    }

    void SetupUI()
    {
        checkResourcesButton.onClick.RemoveAllListeners();
        checkResourcesButton.onClick.AddListener(() =>
        {
            UpdateResourcesDisplay();
            UpdateMonologue("Checking our supplies...");
        });
    }

    void UpdateResourcesDisplay()
    {
        if (resourcesText != null && ResourcesManager.Instance != null)
        {
            resourcesText.text = $"Fuel: {ResourcesManager.Instance.Fuel}\n" +
                                $"Ammo: {ResourcesManager.Instance.Ammunition}\n" +
                                $"Scrap: {ResourcesManager.Instance.ScrapMetal}\n" +
                                $"Survivors: {ResourcesManager.Instance.Survivors}\n" +
                                $"Health: {ResourcesManager.Instance.Health}";
        }
    }

    void UpdateMonologue(string customText = null)
    {
        if (monologueText == null) return;
        if (customText != null)
        {
            monologueText.text = customText;
            return;
        }
        if (ResourcesManager.Instance.Survivors > 0 && Random.value > 0.5f)
        {
            monologueText.text = survivorMonologues[Random.Range(0, survivorMonologues.Length)];
        }
        else
        {
            monologueText.text = playerMonologues[Random.Range(0, playerMonologues.Length)];
        }
    }
}
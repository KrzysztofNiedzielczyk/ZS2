using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    private static CombatManager _instance;
    public static CombatManager Instance => _instance ??= FindFirstObjectByType<CombatManager>() ?? new GameObject("CombatManager").AddComponent<CombatManager>();

    public float preparationTime = 25f;
    public float placementRadius = 12f;
    public GameObject[] defencePrefabs;
    public bool isPreparing = false;
    private float timeLeft;
    private DefenceItem currentDefence;
    private int placedDefences = 0;
    public int maxDefences = 6;
    private int enemiesRemaining;

    [Header("Wave Settings")]
    public float waveInterval = 10f;
    private int totalZombieCount;
    private int[] waveCounts = new int[3];
    private Vector3[] spawnPoints;

    [Header("Zombie Prefabs")]
    public GameObject normalZombiePrefab;
    public GameObject weaponZombiePrefab;
    public GameObject specialZombiePrefab;
    public GameObject exploderZombiePrefab;
    public GameObject throwerZombiePrefab;
    public GameObject shieldedZombiePrefab;

    [Header("Zombie Spawn Chances")]
    public float difficultyThresholdWeapon = 1.05f;
    public float difficultyThresholdSpecial = 1.05f;
    public float difficultyThresholdExploder = 1.15f;
    public float difficultyThresholdThrower = 1.30f;
    public float difficultyThresholdShielded = 1.30f;

    private const int IGNORE_RAYCAST_LAYER = 2;
    private const int ENVIRONMENT_LAYER = 7;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        EventManager.EnemyDied += OnEnemyDied;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameEventManager.Instance != null && GameEventManager.Instance.IsCombatSceneNext())
        {
            StartPreparation();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EventManager.EnemyDied -= OnEnemyDied;
    }

    public void StartPreparation()
    {
        if (TruckManager.Instance == null || Camera.main == null)
        {
            Debug.LogError("Brak TruckManager lub Camera.main!");
            return;
        }

        isPreparing = true;
        timeLeft = preparationTime;
        placedDefences = 0;
        ResourcesManager.Instance.PrepareSurvivorDefences();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowDefencePanel();
        }

        StartCoroutine(PreparationPhase());
    }

    IEnumerator PreparationPhase()
    {
        while (timeLeft > 0 && isPreparing)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        EndPreparation();
    }

    void EndPreparation()
    {
        isPreparing = false;
        if (currentDefence != null) Destroy(currentDefence.gameObject);
        if (UIManager.Instance != null) UIManager.Instance.HideDefencePanel();
        StartCombat();
    }

    void StartCombat()
    {
        int baseCount = CombatData.Instance.GetEnemyCount();
        TerrainCell currentLocation = TravelManager.Instance.GetCurrentLocation();
        int terrainBonus = Random.Range(currentLocation.terrainType.minEnemies, currentLocation.terrainType.maxEnemies + 1);
        float contaminationMultiplier = 1f + currentLocation.contaminationLevel;
        float difficultyMultiplier = 1.01f + (GameTimeManager.Instance.GetTotalGameTimeSeconds() / 3600f / 24f * 0.01f);
        totalZombieCount = Mathf.CeilToInt((baseCount + terrainBonus) * contaminationMultiplier * difficultyMultiplier);
        enemiesRemaining = totalZombieCount;

        waveCounts[0] = Mathf.RoundToInt(totalZombieCount * 0.25f);
        waveCounts[1] = Mathf.RoundToInt(totalZombieCount * 0.35f);
        waveCounts[2] = totalZombieCount - waveCounts[0] - waveCounts[1];

        GameObject[] allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (allSpawnPoints.Length == 0)
        {
            Debug.LogError("Brak punktów spawnu z tagiem 'SpawnPoint' w scenie!");
            return;
        }
        int spawnPointCount = Mathf.Max(2, totalZombieCount / 10);
        spawnPointCount = Mathf.Min(spawnPointCount, allSpawnPoints.Length);
        spawnPoints = new Vector3[spawnPointCount];
        for (int i = 0; i < spawnPointCount; i++)
        {
            int randomIndex = Random.Range(i, allSpawnPoints.Length);
            spawnPoints[i] = allSpawnPoints[randomIndex].transform.position;
            GameObject temp = allSpawnPoints[randomIndex];
            allSpawnPoints[randomIndex] = allSpawnPoints[i];
            allSpawnPoints[i] = temp;
        }

        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        for (int wave = 0; wave < 3; wave++)
        {
            SpawnWave(waveCounts[wave]);
            if (wave < 2) yield return new WaitForSeconds(waveInterval);
        }
    }

    void SpawnWave(int count)
    {
        float contaminationLevel = TravelManager.Instance.GetCurrentLocation().contaminationLevel;
        float difficulty = 1.01f + (GameTimeManager.Instance.GetTotalGameTimeSeconds() / 3600f / 24f * 0.01f);

        float normalChance = difficulty < difficultyThresholdWeapon ? 1f :
                             difficulty < difficultyThresholdExploder ? 0.7f :
                             difficulty < difficultyThresholdThrower ? 0.5f : 0.4f;
        float weaponChance = difficulty < difficultyThresholdExploder ? 0f :
                             difficulty < difficultyThresholdThrower ? 0.2f : 0.2f;
        float specialChance = difficulty < difficultyThresholdExploder ? 0f :
                              difficulty < difficultyThresholdThrower ? 0.1f : 0.15f;
        float exploderChance = difficulty < difficultyThresholdExploder ? 0f :
                               difficulty < difficultyThresholdThrower ? 0f : 0.1f;
        float throwerChance = difficulty < difficultyThresholdThrower ? 0f : 0.1f;
        float shieldedChance = difficulty < difficultyThresholdThrower ? 0f : 0.05f;

        int normalCount = Mathf.RoundToInt(count * normalChance);
        int weaponCount = Mathf.RoundToInt(count * weaponChance);
        int specialCount = Mathf.RoundToInt(count * specialChance);
        int exploderCount = Mathf.RoundToInt(count * exploderChance);
        int throwerCount = Mathf.RoundToInt(count * throwerChance);
        int shieldedCount = Mathf.RoundToInt(count * shieldedChance);
        int remaining = count - (normalCount + weaponCount + specialCount + exploderCount + throwerCount + shieldedCount);
        normalCount += remaining;

        int[] spawnPerPoint = new int[spawnPoints.Length];
        for (int i = 0; i < count; i++)
        {
            spawnPerPoint[i % spawnPoints.Length]++;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            for (int j = 0; j < spawnPerPoint[i]; j++)
            {
                Vector3 spawnPos = spawnPoints[i] + Random.insideUnitSphere * 0.5f;
                float rand = Random.value;
                GameObject zombie;

                if (normalCount > 0 && rand < normalChance)
                {
                    zombie = Instantiate(normalZombiePrefab, spawnPos, Quaternion.identity);
                    normalCount--;
                }
                else if (weaponCount > 0 && rand < normalChance + weaponChance)
                {
                    zombie = Instantiate(weaponZombiePrefab, spawnPos, Quaternion.identity);
                    weaponCount--;
                }
                else if (specialCount > 0 && rand < normalChance + weaponChance + specialChance)
                {
                    zombie = Instantiate(specialZombiePrefab, spawnPos, Quaternion.identity);
                    specialCount--;
                }
                else if (exploderCount > 0 && rand < normalChance + weaponChance + specialChance + exploderChance)
                {
                    zombie = Instantiate(exploderZombiePrefab, spawnPos, Quaternion.identity);
                    exploderCount--;
                }
                else if (throwerCount > 0 && rand < normalChance + weaponChance + specialChance + exploderChance + throwerChance)
                {
                    zombie = Instantiate(throwerZombiePrefab, spawnPos, Quaternion.identity);
                    throwerCount--;
                }
                else
                {
                    zombie = Instantiate(shieldedZombiePrefab, spawnPos, Quaternion.identity);
                    shieldedCount--;
                }
                zombie.layer = 9;
            }
        }
    }

    void OnEnemyDied(GameObject enemy)
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
            EndCombat();
        }
    }

    void EndCombat()
    {
        isPreparing = false;
        placedDefences = 0;
        timeLeft = preparationTime;
        if (currentDefence != null) Destroy(currentDefence.gameObject);
        if (UIManager.Instance != null) UIManager.Instance.HideDefencePanel();
        GameEventManager.Instance.ResetCombatFlag();
        StartCoroutine(EndCombatDelay());
    }

    IEnumerator EndCombatDelay()
    {
        yield return new WaitForSeconds(3f);
        PlayerPrefs.SetInt("FromCombat", 1);
        SceneManager.LoadScene("Main Scene");
    }

    public void SelectDefence(int defenceIndex)
    {
        if (placedDefences >= maxDefences)
        {
            Debug.Log("Osi¹gniêto maksymaln¹ liczbê struktur!");
            return;
        }

        if (!isPreparing)
        {
            Debug.Log("Nie mo¿na wybraæ struktury – faza przygotowañ nieaktywna!");
            return;
        }

        DefenceType type = (DefenceType)defenceIndex;
        if (CanAffordDefence(type))
        {
            SpendResources(type);
            Vector3 initialPos = TruckManager.Instance.transform.position + Vector3.forward * 2f;
            GameObject defenceObject = Instantiate(defencePrefabs[defenceIndex], initialPos, defencePrefabs[defenceIndex].transform.rotation);
            currentDefence = defenceObject.GetComponent<DefenceItem>();
            if (currentDefence == null)
            {
                Debug.LogError("Prefab " + defencePrefabs[defenceIndex].name + " nie ma komponentu DefenceItem!");
                Destroy(defenceObject);
                return;
            }

            int originalLayer = defenceObject.layer;
            defenceObject.layer = IGNORE_RAYCAST_LAYER;
            SetLayerRecursively(defenceObject, IGNORE_RAYCAST_LAYER);

            Collider[] colliders = defenceObject.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            StartCoroutine(PlaceDefenceCoroutine(colliders, originalLayer));
        }
        else
        {
            Debug.Log("Niewystarczaj¹ce zasoby do postawienia struktury: " + type);
        }
    }

    bool CanAffordDefence(DefenceType type)
    {
        int scrapCost = type == DefenceType.FireBarrel ? 5 : type == DefenceType.Spotlight ? 10 : type == DefenceType.Mine ? 0 :
                        type == DefenceType.BarbedWire ? 15 : type == DefenceType.Barricade ? 15 : type == DefenceType.ExplosiveBarrel ? 20 : 15;
        int fuelCost = type == DefenceType.Spotlight ? 10 : type == DefenceType.ExplosiveBarrel ? 5 : 0;
        int ammoCost = type == DefenceType.Mine ? 30 : type == DefenceType.ExplosiveBarrel ? 10 : 0;

        return ResourcesManager.Instance.ScrapMetal >= scrapCost && ResourcesManager.Instance.Fuel >= fuelCost && ResourcesManager.Instance.Ammunition >= ammoCost;
    }

    void SpendResources(DefenceType type)
    {
        int scrapCost = type == DefenceType.FireBarrel ? 5 : type == DefenceType.Spotlight ? 10 : type == DefenceType.Mine ? 0 :
                        type == DefenceType.BarbedWire ? 15 : type == DefenceType.Barricade ? 15 : type == DefenceType.ExplosiveBarrel ? 20 : 15;
        int fuelCost = type == DefenceType.Spotlight ? 10 : type == DefenceType.ExplosiveBarrel ? 5 : 0;
        int ammoCost = type == DefenceType.Mine ? 30 : type == DefenceType.ExplosiveBarrel ? 10 : 0;

        ResourcesManager.Instance.SubtractScrapMetal(scrapCost);
        ResourcesManager.Instance.SubtractFuel(fuelCost);
        ResourcesManager.Instance.SubtractAmmunition(ammoCost);
    }

    IEnumerator PlaceDefenceCoroutine(Collider[] defenceColliders, int originalLayer)
    {
        while (isPreparing && currentDefence != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            int layerMask = 1 << ENVIRONMENT_LAYER;
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                Vector3 placementPos = hit.point;
                float distanceFromTruck = Vector3.Distance(placementPos, TruckManager.Instance.transform.position);

                currentDefence.transform.position = placementPos;

                if (Keyboard.current.qKey.isPressed) currentDefence.transform.Rotate(0, -90 * Time.deltaTime, 0);
                if (Keyboard.current.eKey.isPressed) currentDefence.transform.Rotate(0, 90 * Time.deltaTime, 0);

                // Anulowanie rozstawiania klawiszem ESC
                if (Keyboard.current.escapeKey.wasPressedThisFrame && currentDefence != null)
                {
                    Destroy(currentDefence.gameObject);
                    currentDefence = null;
                    Debug.Log("Rozstawianie struktury anulowane (ESC)");
                    yield break; // Zakoñcz korutynê
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    bool withinRadius = distanceFromTruck <= placementRadius;
                    bool noCollision = !IsColliding(placementPos, currentDefence);

                    if (withinRadius && noCollision)
                    {
                        SetLayerRecursively(currentDefence.gameObject, originalLayer);
                        foreach (var collider in defenceColliders)
                        {
                            collider.enabled = true;
                        }
                        currentDefence.isPlaced = true;
                        placedDefences++;
                        currentDefence = null;
                    }
                    else
                    {
                        Debug.Log("Nie mo¿na zatwierdziæ – powód: " + (withinRadius ? "Kolizja" : "Poza zasiêgiem"));
                    }
                }

                if (Mouse.current.rightButton.wasPressedThisFrame && currentDefence != null)
                {
                    Destroy(currentDefence.gameObject);
                    currentDefence = null;
                }
            }
            else
            {
                Debug.LogError("Raycast nie trafi³ w warstwê Environment – upewnij siê, ¿e teren ma warstwê 7!");
            }

            yield return null;
        }
    }

    bool IsColliding(Vector3 position, DefenceItem ignoreDefence)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f);
        foreach (var col in colliders)
        {
            DefenceItem defenceItem = col.GetComponent<DefenceItem>();
            if ((col.CompareTag("Truck") || (defenceItem != null && defenceItem != ignoreDefence && defenceItem.isPlaced)) && defenceItem != currentDefence)
            {
                return true;
            }
        }
        return false;
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
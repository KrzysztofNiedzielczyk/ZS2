using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>, IDamageable
{
    public enum ResourcesLevelEnum { Critical, Scarce, Sufficient, Abundant, Overflowing }
    public ResourcesLevelEnum AmmunitionLevel;
    public float AmmunitionCurrentBonus = 1;
    public float Health;
    public float Fuel;
    public int Survivors;
    public float Ammunition;
    public float ScrapMetal;
    public float MaxHealth = 1000;
    public int MaxSurvivors = 3;
    public float OverflowingLevelAmmunition = 5000;
    public float OverflowingLevelFuel = 5000;
    public float OverflowingLevelScrapMetal = 5000;
    public float SleepDebt { get; private set; } = 100f;
    public float MaxSleepDebt = 100f;
    public float SleepDebtDecreasePerGameHour = 1f;
    public enum SleepDebtLevel { Rested, Tired, Exhausted, Critical }
    public SleepDebtLevel CurrentSleepDebtLevel { get; private set; } = SleepDebtLevel.Rested;
    private float lastGameTimeSeconds;

    public enum SurvivorClass { None, Engineer, Survivalist, Saboteur }
    public SurvivorClass[] survivorClasses = new SurvivorClass[3];

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Singleton ju¿ to robi, ale dla pewnoœci
    }

    private void Start()
    {
        Health = MaxHealth;
        Survivors = 0;
        SleepDebt = MaxSleepDebt;
        lastGameTimeSeconds = GameTimeManager.Instance.GetTotalGameTimeSeconds();
        survivorClasses[0] = SurvivorClass.Engineer; // Testowe przypisanie
        survivorClasses[1] = SurvivorClass.Survivalist;
        survivorClasses[2] = SurvivorClass.Saboteur;
    }

    private void Update()
    {
        AmmunitionLevelToggle();
        UpdateSleepDebt();
    }

    private void UpdateSleepDebt()
    {
        float currentGameTimeSeconds = GameTimeManager.Instance.GetTotalGameTimeSeconds();
        float timeDeltaGameHours = (currentGameTimeSeconds - lastGameTimeSeconds) / 3600f;

        SleepDebt -= SleepDebtDecreasePerGameHour * timeDeltaGameHours;
        SleepDebt = Mathf.Clamp(SleepDebt, 0f, MaxSleepDebt);

        if (SleepDebt >= 70f) CurrentSleepDebtLevel = SleepDebtLevel.Rested;
        else if (SleepDebt >= 40f) CurrentSleepDebtLevel = SleepDebtLevel.Tired;
        else if (SleepDebt >= 10f) CurrentSleepDebtLevel = SleepDebtLevel.Exhausted;
        else CurrentSleepDebtLevel = SleepDebtLevel.Critical;

        if (SleepDebt <= 0f)
        {
            ForceSleep();
        }

        lastGameTimeSeconds = currentGameTimeSeconds;
    }

    public void IncreaseSleepDebt(float amount)
    {
        SleepDebt = Mathf.Min(MaxSleepDebt, SleepDebt + amount);
    }

    private void ForceSleep()
    {
        Debug.Log("Sleep Debt critical! Forcing sleep...");
        SleepDebt = MaxSleepDebt;
        GameTimeManager.Instance.AdvanceTime(6f);
        lastGameTimeSeconds = GameTimeManager.Instance.GetTotalGameTimeSeconds();
    }

    public bool TrySleep(float hours)
    {
        if (CanSleep())
        {
            GameTimeManager.Instance.AdvanceTime(hours);
            IncreaseSleepDebt(hours * 10f);
            lastGameTimeSeconds = GameTimeManager.Instance.GetTotalGameTimeSeconds();
            Debug.Log($"Slept for {hours} hours. Sleep Debt increased to {SleepDebt}");
            return true;
        }
        Debug.Log("Cannot sleep now!");
        return false;
    }

    private bool CanSleep()
    {
        return true;
    }

    private void AmmunitionLevelToggle()
    {
        if (Ammunition < OverflowingLevelAmmunition * 0.2)
        {
            AmmunitionLevel = ResourcesLevelEnum.Critical;
            AmmunitionCurrentBonus = 0.5f;
        }
        else if (Ammunition < OverflowingLevelAmmunition * 0.4)
        {
            AmmunitionLevel = ResourcesLevelEnum.Scarce;
            AmmunitionCurrentBonus = 0.75f;
        }
        else if (Ammunition < OverflowingLevelAmmunition * 0.6)
        {
            AmmunitionLevel = ResourcesLevelEnum.Sufficient;
            AmmunitionCurrentBonus = 1f;
        }
        else if (Ammunition < OverflowingLevelAmmunition * 0.8)
        {
            AmmunitionLevel = ResourcesLevelEnum.Abundant;
            AmmunitionCurrentBonus = 1.5f;
        }
        else
        {
            AmmunitionLevel = ResourcesLevelEnum.Overflowing;
            AmmunitionCurrentBonus = 2f;
        }
    }

    // Implementacja IDamageable
    public void TakeDamage(float amount)
    {
        Health -= amount;
        Debug.Log($"Ciê¿arówka otrzyma³a {amount} obra¿eñ. Pozosta³e HP: {Health}/{MaxHealth}");

        if (Health <= 0)
        {
            Health = 0;
            Debug.Log("Ciê¿arówka zosta³a zniszczona!");
            // Tu mo¿esz dodaæ logikê koñca gry, np.:
            // SceneManager.LoadScene("GameOver");
        }
    }

    public void AddFuel(int _amount)
    {
        Fuel += _amount;
    }

    public void SubtractFuel(int _amount)
    {
        if (Fuel > 0)
        {
            Fuel -= _amount;
        }
    }

    public void AddSurvivors(int _amount)
    {
        Survivors = Mathf.Min(MaxSurvivors, Survivors + _amount);
        Debug.Log($"Added survivors. Current count: {Survivors}/{MaxSurvivors}");
    }

    public void SubtractSurvivors(int _amount)
    {
        if (Survivors > 0)
        {
            Survivors = Mathf.Max(0, Survivors - _amount);
            Debug.Log($"Lost survivors. Current count: {Survivors}/{MaxSurvivors}");
        }
    }

    public void AddAmmunition(int _amount)
    {
        Ammunition += _amount;
    }

    public void SubtractAmmunition(int _amount)
    {
        if (Ammunition > 0)
        {
            Ammunition -= _amount;
        }
    }

    public void AddScrapMetal(int _amount)
    {
        ScrapMetal += _amount;
    }

    public void SubtractScrapMetal(int _amount)
    {
        if (ScrapMetal > 0)
        {
            ScrapMetal -= _amount;
        }
    }

    public void PrepareSurvivorDefences()
    {
        CombatManager combatManager = FindFirstObjectByType<CombatManager>();
        for (int i = 0; i < Survivors; i++)
        {
            switch (survivorClasses[i])
            {
                case SurvivorClass.Survivalist:
                    combatManager.SelectDefence((int)DefenceType.FireBarrel);
                    break;
                case SurvivorClass.Engineer:
                    combatManager.SelectDefence((int)DefenceType.Barricade);
                    break;
                case SurvivorClass.Saboteur:
                    combatManager.SelectDefence((int)DefenceType.ExplosiveBarrel);
                    break;
            }
        }
    }
}
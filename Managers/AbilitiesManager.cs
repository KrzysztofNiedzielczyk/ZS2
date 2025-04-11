using RengeGames.HealthBars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AbilitiesManager : Singleton<AbilitiesManager>
{
    public bool AbilitiesMenuOn = false;

    public Dictionary<int, Ability> Abilities = new Dictionary<int, Ability>();
    private int currentAbilityToChoose = 1;
    private bool abilitiesChosen = false;
    public GameObject AbilitiesPanel;
    public GameObject Player;

    public Image AbilityOneImage;
    public Image AbilityTwoImage;
    public Image AbilityThreeImage;
    public Image AbilityFourImage;

    [SerializeField] private bool abilityOneReffiling = false;
    [SerializeField] private bool abilityTwoReffiling = false;
    [SerializeField] private bool abilityThreeReffiling = false;
    [SerializeField] private bool abilityFourReffiling = false;

    public AudioSource ButtonPressAudio;


    private void Start()
    {
        AbilitiesMenuToggle();
    }

    private void Update()
    {
        if (abilitiesChosen == false && currentAbilityToChoose > 4)
        {
            abilitiesChosen = true;
            AbilitiesPanel.SetActive(false);
            Time.timeScale = 1;
        }

        AbilityOneUpdate();
        AbilityTwoUpdate();
        AbilityThreeUpdate();
        AbilityFourUpdate();
    }

    void AbilitiesMenuToggle()
    {
        if (AbilitiesMenuOn == true)
        {
            Time.timeScale = 0;
            AbilitiesPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            AbilitiesPanel.SetActive(false);
        }
    }

    void AbilityOneUpdate()
    {
        if (abilityOneReffiling == true)
        {
            AbilityOneImage.fillAmount += (1 / Abilities[1].DelayValue * ResourcesManager.Instance.AmmunitionCurrentBonus) * Time.deltaTime;
        }

        if (AbilityOneImage.fillAmount <= 0)
        {
            abilityOneReffiling = true;
        }
        if (AbilityOneImage.fillAmount >= 1)
        {
            abilityOneReffiling = false;
        }
    }

    void AbilityTwoUpdate()
    {
        if (abilityTwoReffiling == true)
        {
            AbilityTwoImage.fillAmount += (1 / Abilities[2].DelayValue * ResourcesManager.Instance.AmmunitionCurrentBonus) * Time.deltaTime;
        }

        if (AbilityTwoImage.fillAmount <= 0)
        {
            abilityTwoReffiling = true;
        }
        if (AbilityTwoImage.fillAmount >= 1)
        {
            abilityTwoReffiling = false;
        }
    }

    void AbilityThreeUpdate()
    {
        if (abilityThreeReffiling == true)
        {
            AbilityThreeImage.fillAmount += (1 / Abilities[3].DelayValue * ResourcesManager.Instance.AmmunitionCurrentBonus) * Time.deltaTime;
        }

        if (AbilityThreeImage.fillAmount <= 0)
        {
            abilityThreeReffiling = true;
        }
        if (AbilityThreeImage.fillAmount >= 1)
        {
            abilityThreeReffiling = false;
        }
    }

    void AbilityFourUpdate()
    {
        if (abilityFourReffiling == true)
        {
            AbilityFourImage.fillAmount += (1 / Abilities[4].DelayValue * ResourcesManager.Instance.AmmunitionCurrentBonus) * Time.deltaTime;
        }

        if (AbilityFourImage.fillAmount <= 0)
        {
            abilityFourReffiling = true;
        }
        if (AbilityFourImage.fillAmount >= 1)
        {
            abilityFourReffiling = false;
        }
    }

    public void ActivateAbilityOne(InputAction.CallbackContext context)
    {
        if (Abilities[1].Delay <= 0)
        {
            // activate ability
            Abilities[1].OnAbilityActivation(context);

            // TO-DO use AbilityOneBar to set value to 0 than start ability delay and fill the bar accordingly
            AbilityOneImage.fillAmount = 0;
        }
    }

    public void ActivateAbilityTwo(InputAction.CallbackContext context)
    {
        if (Abilities[2].Delay <= 0)
        {
            Abilities[2].OnAbilityActivation(context);
            AbilityTwoImage.fillAmount = 0;
        }
    }

    public void ActivateAbilityThree(InputAction.CallbackContext context)
    {
        if (Abilities[3].Delay <= 0)
        {
            Abilities[3].OnAbilityActivation(context);
            AbilityThreeImage.fillAmount = 0;
        }
    }

    public void ActivateAbilityFour(InputAction.CallbackContext context)
    {
        if (Abilities[4].Delay <= 0)
        {
            Abilities[4].OnAbilityActivation(context);
            AbilityFourImage.fillAmount = 0;
        }
    }

    public void AddFlashbang()
    {
        Abilities.Add(currentAbilityToChoose, Player.GetComponent<FlashbangAbilityHandler>());
        currentAbilityToChoose++;
        ButtonPressAudio.Stop();
        ButtonPressAudio.Play();
    }

    public void AddFragGrenade()
    {
        Abilities.Add(currentAbilityToChoose, Player.GetComponent<FragGrenadeAbilityHandler>());
        currentAbilityToChoose++;
        ButtonPressAudio.Stop();
        ButtonPressAudio.Play();
    }

    public void AddMolotovCocktail()
    {
        Abilities.Add(currentAbilityToChoose, Player.GetComponent<MolotovCocktailAbilityHandler>());
        currentAbilityToChoose++;
        ButtonPressAudio.Stop();
        ButtonPressAudio.Play();
    }

    public void AddShotgunBlast()
    {
        Abilities.Add(currentAbilityToChoose, Player.GetComponent<ShotgunBlastHandler>());
        currentAbilityToChoose++;
        ButtonPressAudio.Stop();
        ButtonPressAudio.Play();
    }

    public void AddAdrenalineShot()
    {
        Abilities.Add(currentAbilityToChoose, Player.GetComponent<AdrenalineBoostHandler>());
        currentAbilityToChoose++;
        ButtonPressAudio.Stop();
        ButtonPressAudio.Play();
    }

    public void AddArtilleryBarrage()
    {
        Abilities.Add(currentAbilityToChoose, Player.GetComponent<ArtilleryHandler>());
        currentAbilityToChoose++;
        ButtonPressAudio.Stop();
        ButtonPressAudio.Play();
    }

    public void AddRPGShot()
    {
        Abilities.Add(currentAbilityToChoose, Player.GetComponent<RPGShotHandler>());
        currentAbilityToChoose++;
        ButtonPressAudio.Stop();
        ButtonPressAudio.Play();
    }

    public void AddHeavySniperShot()
    {
        Abilities.Add(currentAbilityToChoose, Player.GetComponent<HeavySniperShotHandler>());
        currentAbilityToChoose++;
        ButtonPressAudio.Stop();
        ButtonPressAudio.Play();
    }

    public void AddEnergyPulse()
    {
        Abilities.Add(currentAbilityToChoose, Player.GetComponent<EnergyPulseHandler>());
        currentAbilityToChoose++;
        ButtonPressAudio.Stop();
        ButtonPressAudio.Play();
    }

    public void AddRocketVolley()
    {
        Abilities.Add(currentAbilityToChoose, Player.GetComponent<RocketBarrageAbilityHandler>());
        currentAbilityToChoose++;
        ButtonPressAudio.Stop();
        ButtonPressAudio.Play();
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
                if (_instance == null)
                {
                    GameObject controllerObj = new GameObject("GameController");
                    _instance = controllerObj.AddComponent<GameController>();
                }
            }
            return _instance;
        }
    }

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
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateReferences();
    }

    private CameraMovementHandler cameraMovementHandler;

    private void UpdateReferences()
    {
        cameraMovementHandler = FindObjectOfType<CameraMovementHandler>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (cameraMovementHandler != null)
        {
            cameraMovementHandler.OnMove(context);
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (cameraMovementHandler != null)
        {
            cameraMovementHandler.OnZoom(context);
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (Player.Instance != null && Player.Instance.WeaponShooting != null)
        {
            Player.Instance.WeaponShooting.OnFire(context);
        }
    }

    // Obs�uga czterech umiej�tno�ci
    public void OnAbility1(InputAction.CallbackContext context)
    {
        ActivateAbility(0, context); // Pierwsza zdolno��
    }

    public void OnAbility2(InputAction.CallbackContext context)
    {
        ActivateAbility(1, context); // Druga zdolno��
    }

    public void OnAbility3(InputAction.CallbackContext context)
    {
        ActivateAbility(2, context); // Trzecia zdolno��
    }

    public void OnAbility4(InputAction.CallbackContext context)
    {
        ActivateAbility(3, context); // Czwarta zdolno��
    }

    private void ActivateAbility(int abilityIndex, InputAction.CallbackContext context)
    {
        if (Player.Instance != null)
        {
            var abilities = Player.Instance.GetComponents<Ability>();
            if (abilities != null && abilityIndex >= 0 && abilityIndex < abilities.Length)
            {
                abilities[abilityIndex].OnAbilityActivation(context);
            }
            else
            {
                Debug.LogWarning($"Ability at index {abilityIndex} not found on Player!");
            }
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class TruckPlayer : MonoBehaviour
{
    public static TruckPlayer Instance { get; private set; }
    private PlayerInput playerInput;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);

        if (GameController.Instance != null)
        {
            playerInput = GameController.Instance.GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("GameController nie ma komponentu PlayerInput!");
            }
        }
        else
        {
            Debug.LogError("GameController.Instance jest null!");
        }
    }

    void Start()
    {
        transform.position = TruckManager.Instance.transform.position + new Vector3(0, 1f, 0);
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed) GamePauseMenu.Instance.FlipPauseMenu();
    }
}
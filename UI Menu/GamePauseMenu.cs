using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GamePauseMenu : MonoBehaviour
{
    // Dodajemy Singleton
    private static GamePauseMenu _instance;
    public static GamePauseMenu Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GamePauseMenu>();
                if (_instance == null)
                {
                    GameObject pauseMenuObj = new GameObject("GamePauseMenu");
                    _instance = pauseMenuObj.AddComponent<GamePauseMenu>();
                }
            }
            return _instance;
        }
    }

    public GameObject PauseMenu;
    public GameObject SettingsMenu;
    public GameObject ActionsMenu;
    public GameObject BuildMenu;
    public GameObject ScavengeMenu;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // Zachowujemy obiekt miêdzy scenami
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            FlipPauseMenu();
        }
    }

    public void FlipPauseMenu()
    {
        if (TimeManager.Instance.IsGamePaused == true)
        {
            if (SettingsMenu.activeInHierarchy)
            {
                SettingsMenu.SetActive(false);
                PauseMenu.SetActive(true);
                return;
            }
            if (BuildMenu.activeInHierarchy)
            {
                BuildMenu.SetActive(false);
                ActionsMenu.SetActive(true);
                return;
            }
            if (ScavengeMenu.activeInHierarchy)
            {
                ScavengeMenu.SetActive(false);
                ActionsMenu.SetActive(true);
                return;
            }
            if (ActionsMenu.activeInHierarchy)
            {
                ActionsMenu.SetActive(false);
                TimeManager.Instance.PauseFlip();
                return;
            }

            TimeManager.Instance.PauseFlip();
            PauseMenu.SetActive(false);
        }
        else
        {
            TimeManager.Instance.PauseFlip();
            PauseMenu.SetActive(true);
        }
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("Main_Menu");
    }
}
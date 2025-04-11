using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject GameOverButton;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            GameOverButton.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }

    public void GameOver()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}

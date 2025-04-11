using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    public bool IsGamePaused = false;

    public void PauseFlip()
    {
        if (IsGamePaused == true)
        {
            Time.timeScale = 1;
            IsGamePaused = false;
        }
        else
        {
            Time.timeScale = 0;
            IsGamePaused = true;
        }
    }
}

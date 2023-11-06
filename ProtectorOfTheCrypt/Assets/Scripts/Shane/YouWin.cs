using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class YouWin : MonoBehaviour
{
    public TextMeshProUGUI killCount;
    void Start()
    {
        if (GameManager.instance.GameMode is EndlessMode)
        {
            killCount.text = "Enemies Defeated: " + KillCounter.enemiesKilled;
        }
        else
        {
            killCount.text = "";
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");                        //Load Main Menu Scene
    }
}


public static class KillCounter
{
    public static int enemiesKilled;
}

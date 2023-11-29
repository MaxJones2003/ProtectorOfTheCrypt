using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmQuit : MonoBehaviour
{
    public GameObject confirmQuitPanel;

    public static bool closeApp;            // IF TRUE, CLOSE GAME. IF FALSE, RETURN TO MAIN MENU

    public void CloseGame()
    {
        if (closeApp)
        {
            Application.Quit();
        }
        else if (!closeApp)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    public void Cancel()
    {
        confirmQuitPanel.SetActive(false);
    }
}

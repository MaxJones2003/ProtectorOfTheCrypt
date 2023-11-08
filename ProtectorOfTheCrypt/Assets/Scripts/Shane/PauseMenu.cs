using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject IGButtons;
    public TextMeshProUGUI seedDisplay;
    private String seedHolder;

    void Start()
    {
        if (GameManager.instance.GameMode is EndlessMode)
        {
            seedHolder = Seed.Instance.GameSeed;
        }

        if (seedHolder != null)
        {
            seedDisplay.text = "Seed: " + seedHolder;
        }
        else
        {
            seedDisplay.text = "";
        }
    }

    public void Pause()
    {
        PausePanel.SetActive(true);                                     //Pause Menu displays
        GameManager.instance.GamePaused(true);                          //Stops game
        Time.timeScale = 0;                                             //Pauses Game Speed
        IGButtons.GetComponent<UIButtons>().UIButton.SetActive(false);  //Stops in game UI buttons disappear via UIButtons.cs
    }

    public void Continue()
    {
        PausePanel.SetActive(false);                                    //Pause menu goes away
        GameManager.instance.GamePaused(false);                         //Game resumes
        Time.timeScale = 1;                                             //Re-enables Game Speed
        IGButtons.GetComponent<UIButtons>().UIButton.SetActive(true);   //Makes in game UI buttons appear via UIButtons.cs
    }

    public void MainMenu()
    {
        Time.timeScale = 1;                                             //Re-enables game speed
        SceneManager.LoadScene("MainMenuScene");                        //Load Main Menu Scene
    }

    public void QuitApp()
    {
        Application.Quit();                         //Completely Close Application
    }
}

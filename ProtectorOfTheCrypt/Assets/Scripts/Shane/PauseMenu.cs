using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject IGButtons;

    // Update is called once per frame
    void Update()
    {

    }

    public void Pause()
    {
        PausePanel.SetActive(true);                                     //Pause Menu displays
        Time.timeScale = 0;                                             //Stops game
        IGButtons.GetComponent<UIButtons>().UIButton.SetActive(false);  //Stops in game UI buttons disappear via UIButtons.cs
    }

    public void Continue()
    {
        PausePanel.SetActive(false);                                    //Pause menu goes away
        Time.timeScale = 1;                                             //Game resumes
        IGButtons.GetComponent<UIButtons>().UIButton.SetActive(true);   //Makes in game UI buttons appear via UIButtons.cs
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");                        //Load Main Menu Scene
    }
}

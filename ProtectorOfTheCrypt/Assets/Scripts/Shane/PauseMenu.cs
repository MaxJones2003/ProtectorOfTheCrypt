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
        GameManager.instance.GamePaused(true);                                         //Stops game
        IGButtons.GetComponent<UIButtons>().UIButton.SetActive(false);  //Stops in game UI buttons disappear via UIButtons.cs
    }

    public void Continue()
    {
        PausePanel.SetActive(false);                                    //Pause menu goes away
        GameManager.instance.GamePaused(false);                                          //Game resumes
        IGButtons.GetComponent<UIButtons>().UIButton.SetActive(true);   //Makes in game UI buttons appear via UIButtons.cs
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");                        //Load Main Menu Scene
    }
}

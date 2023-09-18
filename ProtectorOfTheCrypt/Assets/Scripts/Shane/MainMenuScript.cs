using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject MainPanel;
    public CreditsMenu CredRef;
    public SettingsMenu SettRef;
    public void StoryMode()
    {
        SceneManager.LoadScene("SampleScene");      //Load StoryMode Scene
    }

    public void TimedMode()
    {
        SceneManager.LoadScene("SampleScene");      //Load Timed Mode Scene
    }

    public void Settings()
    {
        MainPanel.SetActive(false);                 //Make Main Menu Inactive
        SettRef.SettingsPanel.SetActive(true);      //Make Settings Menu Active
    }

    public void Credits()
    {
        MainPanel.SetActive(false);                 //Make Main Menu Inactive
        CredRef.CreditsPanel.SetActive(true);       //Make Credits Panel Active
    }

    public void QuitGame()
    {
        Application.Quit();                         //Completely Close Application
    }
}

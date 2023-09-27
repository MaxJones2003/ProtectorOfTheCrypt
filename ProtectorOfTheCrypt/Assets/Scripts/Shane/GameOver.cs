using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOver : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

    }

    // Needs to Restart Active Scene
    public void Retry()
    {
        Time.timeScale = 1;                                             //Starts game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void MainMenu()
    {
        Time.timeScale = 1;                                             //Starts game
        SceneManager.LoadScene("MainMenuScene");                        //Load Main Menu Scene
    }
}

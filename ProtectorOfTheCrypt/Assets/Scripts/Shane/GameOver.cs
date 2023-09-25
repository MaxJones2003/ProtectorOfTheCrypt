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
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
    }


    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");                        //Load Main Menu Scene
    }
}

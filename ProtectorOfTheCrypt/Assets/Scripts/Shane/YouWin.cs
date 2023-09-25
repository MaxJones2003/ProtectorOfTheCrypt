using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class YouWin : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");                        //Load Main Menu Scene
    }
}

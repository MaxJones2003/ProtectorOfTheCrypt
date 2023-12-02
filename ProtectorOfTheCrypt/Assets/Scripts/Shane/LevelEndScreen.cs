using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndScreen : MonoBehaviour
{
    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (SceneManager.GetActiveScene().name == "Level2")
        {
            SceneManager.LoadScene("Level3");
        }
        else if (SceneManager.GetActiveScene().name == "Level3")
        {
            SceneManager.LoadScene("Level4");
        }
        else if (SceneManager.GetActiveScene().name == "Level4")
        {
            SceneManager.LoadScene("Level5");
        }
        else if (SceneManager.GetActiveScene().name == "Level5")
        {
            SceneManager.LoadScene("Level6");
        }
        else if (SceneManager.GetActiveScene().name == "Level6")
        {
            SceneManager.LoadScene("Level7");
        }
        else if (SceneManager.GetActiveScene().name == "Level7")
        {
            SceneManager.LoadScene("Level8");
        }
        else if (SceneManager.GetActiveScene().name == "Level8")
        {
            SceneManager.LoadScene("Level9");
        }
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}

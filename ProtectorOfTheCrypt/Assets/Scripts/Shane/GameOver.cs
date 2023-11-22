using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI killCount;
    public TextMeshProUGUI seed;
    public Button retryButton;

    void Start()
    {
        if (GameManager.instance.GameMode is EndlessMode)
        {
            killCount.text = "Enemies Defeated: " + KillCounter.enemiesKilled;

            seed.text = "Seed: " + Seed.Instance.GameSeed;
            retryButton.gameObject.SetActive(false);                                // Retry button only displays in Story Mode
        }
        else
        {
            killCount.text = "";
            seed.text = "";
        }
    }

    // Needs to Restart Active Scene
    public void Retry()
    {
        Time.timeScale = 1;                                             //Starts game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);     // Reloads Active Scene
        AudioManager.instance.PlayMusicOnSceneChange("SampleScene");
    }


    public void MainMenu()
    {
        Time.timeScale = 1;                                             //Starts game
        SceneManager.LoadScene("MainMenuScene");
        AudioManager.instance.PlayMusicOnSceneChange("MainMenuScene");//Load Main Menu Scene
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryMode : GameMode
{
    public WaveManager waveManager;
    public DialogueController DialogueController;

    public GameObject YouWinScreen;
    public GameObject GameOverScreen;
    public GameObject UIButtons;

    public void Awake()
    {
        DialogueController = GetComponent<DialogueController>();
        Seed.Instance.InitializeSeedScriptStoryMode();
    }

    public override bool CheckGameWon()
    {
        return waveManager.state == WaveManager.SpawnState.FINISHED
               && waveManager.EnemySpawner.SpawnedObjects.Count == 0;
    }

    public override bool CheckGameLost()
    {
        return GameManager.instance.Souls <= 0;
    }

    public override void OnGameLost()
    {
        // Activate Game Over Screen 
        GameOverScreen.SetActive(true);

        // Disable Game UI
        UIButtons.SetActive(false);

        // Freeze Game so that the YouWin bug goes away
        Time.timeScale = 0;                                             //Stops game

    }
    public override void OnGameWon()
    {
        // Activate You Win Screen 
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("test");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            AudioManager.instance.PlayMusicOnSceneChange(SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1).name);
            return;
        }

        YouWinScreen.SetActive(true);
        // Disable Game UI
        UIButtons.SetActive(false);

        // Debug.Log("game won test");
    }
}
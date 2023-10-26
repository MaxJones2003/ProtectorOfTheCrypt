using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndlessMode : GameMode
{
    [SerializeField] private EndlessModeSettings[] PresetSettings;
    public WaveManager waveManager;
    public DialogueController DialogueController;

    public GameObject YouWinScreen;
    public GameObject GameOverScreen;
    public GameObject UIButtons;

    public void Awake()
    {
        DialogueController = GetComponent<DialogueController>();
        ReadyToLoadMap();
    }

    public void ReadyToLoadMap()
    {
        EndlessModeSettings setting = PresetSettings[2];
        Seed.Instance.InitializeSeedScriptEndlessMode(setting);
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
public enum EndlessDifficulty { Easy, Standard, Hard, Custom }

[System.Serializable]
public struct EndlessModeSettings
{
    public EndlessDifficulty difficulty;
    public MapSizeSettings mapSizeSettings;
    public EnemyDifficultySettings enemyDifficultySettings;

    public EndlessModeSettings(MapSizeSettings mapSizeSettings, EnemyDifficultySettings enemyDifficultySettings, EndlessDifficulty difficulty = EndlessDifficulty.Custom)
    {
        this.mapSizeSettings = mapSizeSettings;
        this.enemyDifficultySettings = enemyDifficultySettings;
        this.difficulty = difficulty;
    }
}

[System.Serializable]
public struct MapSizeSettings
{
    public int width, height;
    public int minLength, maxLength;

    public MapSizeSettings(int width, int height, int minLength, int maxLength)
    {
        this.width = width;
        this.height = height;
        this.minLength = minLength;
        this.maxLength = maxLength;
    }
}

[System.Serializable]
public struct EnemyDifficultySettings
{
    public float healthMultiplier;
    public int hungerMultiplier;
    public int enemySpawnAmountMultiplier;

    public EnemyDifficultySettings(float healthMultiplier, int hungerMultiplier, int enemySpawnAmountMultiplier)
    {
        this.healthMultiplier = healthMultiplier;
        this.hungerMultiplier = hungerMultiplier;
        this.enemySpawnAmountMultiplier = enemySpawnAmountMultiplier;
    }
}

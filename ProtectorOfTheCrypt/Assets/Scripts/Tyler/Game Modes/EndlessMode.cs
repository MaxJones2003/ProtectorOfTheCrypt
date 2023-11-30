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

    public EnemyScriptableObject basicEnemy;
    public EnemyScriptableObject shieldEnemy;
    public EnemyScriptableObject goldEnemy;
    public EnemyScriptableObject wizardEnemy;


    public EndlessModeSettings CurrentSettings { get; private set; }

    public void Awake()
    {
        DialogueController = GetComponent<DialogueController>();
    }

    public void ReadyToLoadMap(EndlessModeSettings setting)
    {
        CurrentSettings = setting;
        Seed.Instance.InitializeSeedScriptEndlessMode(setting);
        waveManager.SpawnFirstWave();
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
    public string seed;
    public EndlessDifficulty difficulty;
    public MapSizeSettings mapSizeSettings;
    public EnemyDifficultySettings enemyDifficultySettings;
    public EndlessModeSettings(string seed, MapSizeSettings mapSizeSettings, EnemyDifficultySettings enemyDifficultySettings, EndlessDifficulty difficulty = EndlessDifficulty.Custom)
    {
        this.seed = seed;
        this.mapSizeSettings = mapSizeSettings;
        this.enemyDifficultySettings = enemyDifficultySettings;
        this.difficulty = difficulty;
    }

    public override string ToString()
    {
        return $"String: {seed}, Map Settings: {mapSizeSettings}";
    }
}

[System.Serializable]
public struct MapSizeSettings
{
    public int width, height;
    public int minLength, maxLength;
    [Tooltip("The amount of hazard groups that will be spawned in the map + or - 3")]
    [SerializeField] Vector2Int numOfHazardGroupRange;
    public int hazardGroups 
    {
        get
        {
            return Random.Range(numOfHazardGroupRange.x, numOfHazardGroupRange.y);
        }
    }
    public GameObject cameraPosition;

    public MapSizeSettings(int width, int height, int minLength, int maxLength, GameObject cameraPosition, Vector2Int hazardGroupsRange)
    {
        this.width = width;
        this.height = height;
        this.minLength = minLength;
        this.maxLength = maxLength;
        this.cameraPosition = cameraPosition;
        numOfHazardGroupRange = hazardGroupsRange;
    }

    public override string ToString()
    {
        return $"Width: {width}, Height: {height}";
    }
}

[System.Serializable]
public struct EnemyDifficultySettings
{
    public float healthMultiplier;
    public int hungerMultiplier;
    public float speedMultiplier;
    public int enemySpawnAmountMultiplier;

    public EnemyDifficultySettings(float healthMultiplier, int hungerMultiplier, int speedMultiplier, int enemySpawnAmountMultiplier)
    {
        this.healthMultiplier = healthMultiplier;
        this.hungerMultiplier = hungerMultiplier;
        this.speedMultiplier = speedMultiplier;
        this.enemySpawnAmountMultiplier = enemySpawnAmountMultiplier;
    }
}

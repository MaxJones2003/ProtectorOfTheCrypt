using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class Seed : MonoBehaviour
{
    private static Seed instance;
    public static Seed Instance
    {
        get
        {
            return instance;
        }
    }
    [Header("Load Scene: ")]
    [Tooltip("If the prefab is level1, this int should be 1, if level2 it should be 2.")]
    [SerializeField] private int levelToLoad = 0;
    [SerializeField] private GameObject loadThisPrefabLevel;
    [Header("Seed Information")]
    public bool pickRandomSeed = true;
    [SerializeField] private string GameSeed = "default";
    private int CurrentSeed = 0;

    [Header("Make a new Level")]
    [Tooltip("This value MUST be DIFFERENT than all other level numbers, else it will override another map")]
    [SerializeField] private int LevelNumber;
    [SerializeField] private bool LevelMaking_UseSameSeed = false;


    public GridManager gridManager;    

    private void Awake() 
    {
        instance = this;
    }

    public void InitializeSeedScriptStoryMode()
    {
        LoadCurrentLevel(levelToLoad, loadThisPrefabLevel);
        InitializeRandom();
    }

    public void InitializeSeedScriptEndlessMode(EndlessModeSettings settings)
    {
        GameSeed = CreateRandomSeed(16);
        InitializeRandom();
        MapSizeSettings mapSettings = settings.mapSizeSettings;

        if(transform.GetChild(0).name == "Path") Destroy(transform.GetChild(0).gameObject);

        gridManager.GenerateRandomPath(mapSettings.width, mapSettings.height, mapSettings.minLength, mapSettings.maxLength);
    }
    public void InitializeSeedScriptEndlessModeEditor()
    {
        InitializeRandom();

        gridManager.GenerateRandomPath();
    }
    public void InitializeSeedScriptEndlessModeAndRandomizeSeedEditor()
    {
        GameSeed = CreateRandomSeed(16);
        InitializeRandom();

        gridManager.GenerateRandomPath();
    }

    private void InitializeRandom()
    {
        CurrentSeed = GameSeed.GetHashCode(); // Creates an integer seed based on the GameSeed string
        Random.InitState(CurrentSeed); // Initializes Random to use the seed, this means results using Random will be reproducable via a seed
    }

    private string CreateRandomSeed(int length)
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_!?";
        string generated_string = "";

        for(int i = 0; i < length; i++)
            generated_string += characters[Random.Range(0, length)];

        return generated_string;
    }

    public void SaveCurrentSeed()
    {
        // Get Map width, height, and min path length
        int w = gridManager.gridWidth;
        int h = gridManager.gridHeight;
        int min = gridManager.minPathLength;
        int max = gridManager.maxPathLength;
        List<Vector3> path = gridManager.loadedEnemyPath;

        // Create a new variable containing all the important values
        MapVariables newMap = new MapVariables(LevelNumber, GameSeed, w, h, min, max, path);

        string json = JsonUtility.ToJson(newMap);
        SaveSystem.Save(json, "/Level" + newMap.LevelNumber.ToString());
    }

    public void LoadCurrentLevel(int levelNumber, GameObject path)
    {
        // Figure out what level we're on
        // Will do when we have more than one level

        // Based on the level number, determine the file path of the json to load
        string filePath = "/Level" + levelNumber.ToString();
        string saveString = SaveSystem.Load(filePath);
        if (saveString == null) return;

        // Load Variables from JSON
  
        MapVariables currentMap = JsonUtility.FromJson<MapVariables>(saveString);
        GameSeed = currentMap.Seed;
        gridManager.GenerateLoadedPath(currentMap.GridWidth, currentMap.GridHeight, currentMap.MinPathLength, currentMap.MaxPathLength, path, currentMap.LevelEnemyPath);
        //gridManager.gridWidth = currentMap.GridWidth;
        //gridManager.gridHeight = currentMap.GridHeight;
        //gridManager.minPathLength = currentMap.MinPathLength;
        //gridManager.maxPathLength = currentMap.MaxPathLength;
        //gridManager.loadedPath = path;
        //gridManager.loadedEnemyPath = currentMap.LevelEnemyPath;
        Debug.Log(gridManager.loadedPath);
    }
}
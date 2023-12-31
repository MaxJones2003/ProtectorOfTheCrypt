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
    [SerializeField] public string GameSeed = "default";
    private int CurrentSeed = 0;

    [Header("Make a new Level")]
    [Tooltip("This value MUST be DIFFERENT than all other level numbers, else it will override another map")]
    [SerializeField] private int LevelNumber;
    [SerializeField] private bool LevelMaking_UseSameSeed = false;
    [SerializeField] private bool makeRandomHazards = false;
    [SerializeField] private int minHazardGroup, maxHazardGroup;


    public GridManager gridManager;    

    private void Awake() 
    {
        instance = this;
    }

    public void InitializeSeedScriptStoryMode()
    {
        Debug.Log("Initializing random for story");
        LoadCurrentLevel(levelToLoad, loadThisPrefabLevel);
        InitializeRandom();
    }

    public void InitializeSeedScriptEndlessMode(EndlessModeSettings settings)
    {
        GameSeed = settings.seed;
        InitializeRandom();
        MapSizeSettings mapSettings = settings.mapSizeSettings;

        if(transform.GetChild(0).name == "Path") Destroy(transform.GetChild(0).gameObject);

        gridManager.GenerateRandomPath(mapSettings.width, mapSettings.height, mapSettings.minLength, mapSettings.maxLength, mapSettings.cameraPosition, mapSettings.hazardGroups);
    }
    public void InitializeSeedScriptEndlessModeEditor()
    {
        InitializeRandom();

        gridManager.GenerateRandomPath(makeRandomHazards, minHazardGroup, maxHazardGroup);
    }
    public void InitializeSeedScriptEndlessModeAndRandomizeSeedEditor()
    {
        GameSeed = CreateRandomSeed(16);
        InitializeRandom();

        gridManager.GenerateRandomPath(makeRandomHazards, minHazardGroup, maxHazardGroup);
    }

    private void InitializeRandom()
    {
        CurrentSeed = GameSeed.GetHashCode(); // Creates an integer seed based on the GameSeed string
        Random.InitState(CurrentSeed); // Initializes Random to use the seed, this means results using Random will be reproducable via a seed
    }

    public string CreateRandomSeed(int length, bool useNumbersOnly = false)
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_!?";
        if (useNumbersOnly) characters = "0123456789";
        string generated_string = "";

        for(int i = 0; i < length; i++)
            generated_string += characters[Random.Range(0, characters.Length)];

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
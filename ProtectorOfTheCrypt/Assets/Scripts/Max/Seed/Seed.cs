using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class Seed : MonoBehaviour
{
    public bool pickRandomSeed = true;
    [SerializeField] private string GameSeed = "default";
    private int CurrentSeed = 0;
    [Tooltip("This value MUST be DIFFERENT than all other level numbers, else it will override another map")]
    [SerializeField] private int LevelNumber;

    private GridManager gridManager;

    [Header("List of level prefabs")]
    [SerializeField] private List<GameObject> levelPrefabs;

    [SerializeField] private bool LevelMaking_UseSameSeed = false;

    private void Awake() 
    {
        gridManager = gameObject.GetComponent<GridManager>();

        if(pickRandomSeed)
        {
            if(!LevelMaking_UseSameSeed) 
                GameSeed = CreateRandomSeed(16);
            InitializeRandom();
        }
        else
        {
            LoadCurrentLevel(/*Game Manager should probably keep track of the level somehow*/);
            InitializeRandom();
        }
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

    public void LoadCurrentLevel(/*Might get the current level number from here*/)
    {
        // Figure out what level we're on
        // Will do when we have more than one level
        int levelNumber = 0;

        // Based on the level number, determine the file path of the json to load
        string filePath = "/Level" + levelNumber.ToString();
        string saveString = SaveSystem.Load(filePath);
        if (saveString == null) return;

        // Load Variables from JSON
  
        MapVariables currentMap = JsonUtility.FromJson<MapVariables>(saveString);
        gridManager.gridWidth = currentMap.GridWidth;
        gridManager.gridHeight = currentMap.GridHeight;
        gridManager.minPathLength = currentMap.MinPathLength;
        gridManager.maxPathLength = currentMap.MaxPathLength;
        GameSeed = currentMap.Seed;
        gridManager.loadedPath = levelPrefabs[levelNumber];
        gridManager.loadedEnemyPath = currentMap.LevelEnemyPath;
    }
}
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapVariables
{
    public int LevelNumber;
    public string Seed;
    public int GridWidth, GridHeight, MinPathLength, MaxPathLength;
    public GameObject LevelPrefab;
    public List<Vector3> LevelEnemyPath = new List<Vector3>();

    public MapVariables(int levelNumber, string seed, int gridWidth, int gridHeight, int minPathLength, int maxPathLength, GameObject levelPrefab, List<Vector3> levelEnemyPath)
    {
        LevelNumber = levelNumber;
        Seed = seed;
        GridWidth = gridWidth;
        GridHeight = gridHeight;
        MinPathLength = minPathLength;
        MaxPathLength = maxPathLength;
        LevelPrefab = levelPrefab;
        LevelEnemyPath = levelEnemyPath;
    }
}

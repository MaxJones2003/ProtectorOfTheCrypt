using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 8;
    [SerializeField] private Transform hazards;

    [Tooltip("The lower the Min Path Length, the less Variation.")]
    public int minPathLength = 15;
    [Tooltip("The Max Path Length should be at least double the Grid Width, otherwise you might crash the application.")]
    public int maxPathLength = 40;

    /// <Summary> Enemy Manager relays the information from the grid and gives it to the enemies</Summary>
    private WaveManager WaveManager;

    [Tooltip("Grid Cells, set in inspector, used to place paths based on the path grid")]
    public GridCellScriptableObject[] pathCellObjects;
    [Tooltip("Empty grid cell, used to fill the map with places to set towers")]
    public GridCellScriptableObject emptyCell;
    [Tooltip("Grid Cells, set in inspector, fills empty space that is not used by the path grid")]
    public GridCellScriptableObject[] sceneryCellObjects;

    private PathGenerator pathGenerator;
    public GameObject loadedPath = null;
    public List<Vector3> loadedEnemyPath = new List<Vector3>();


    [SerializeField] private GameObject mapBoundPrefab;
    private void Awake()
    {
        Camera.main.transform.position = Vector3.zero + new Vector3(gridWidth / 2, (gridHeight * 2) / 2, -gridHeight / 3);
        Camera.main.transform.LookAt(new Vector3(gridWidth/2, 0, gridHeight/2 - 4));
        // If the seed script is there, and the seed script's PickRandomSeed value is false:
        // Set the values of the grid/path to those saved in the MapVariables class.
        Seed seedScript = gameObject.GetComponent<Seed>();
        WaveManager = GetComponent<WaveManager>();

        if (seedScript == null)
            return;
        if(seedScript.pickRandomSeed)
            return;

        Camera.main.transform.position = Vector3.zero + new Vector3(gridWidth / 2, (gridHeight * 2) / 2, -gridHeight / 3);
    }
    private void Start()
    {
        pathGenerator = new PathGenerator(gridWidth, gridHeight, hazards);
        
        if(loadedPath == null)
            Generate();
        else
        {
            Instantiate(loadedPath);
            SetUpEnemies(loadedEnemyPath);
        }
    }

    /// <summary>
    /// Starts the generation and building of the path
    /// </summary>
    /// <param name="check"></param>
    private void Generate()
    {
        List<Vector2Int> pathCells = pathGenerator.GeneratePath();
        int pathSize = pathCells.Count;
        int crossroadsAdded = 0;
        while (pathGenerator.GenerateCrossroads() && crossroadsAdded < 2)
        {
            pathSize = pathCells.Count;
            crossroadsAdded++;
        }
        float emptyCellWeight = 75;
        float remaininWeightOutOfHundred = 100 - emptyCellWeight;

        Dictionary<GridCellScriptableObject, float> weightTable = new();
        weightTable.Add(emptyCell, emptyCellWeight);
        foreach (GridCellScriptableObject cell in sceneryCellObjects)
        {
            float weight = remaininWeightOutOfHundred / sceneryCellObjects.Length;
            weightTable.Add(cell, weight);
        }

        while (pathSize < minPathLength || pathSize > maxPathLength)
        {
            pathCells = pathGenerator.GeneratePath();
            crossroadsAdded = 0;
            while (pathGenerator.GenerateCrossroads() && crossroadsAdded < 2)
            {                
                pathSize = pathCells.Count;
                crossroadsAdded++;
            }
        }

        StartCoroutine(CreateGrid(pathCells));
    }

    private IEnumerator CreateGrid(List<Vector2Int> pathCells)
    {
        GameObject parentGO = new GameObject();
        Transform parent = parentGO.transform;
        parent.name = "Path";
        LayPathCells(pathCells, parent);
        LaySceneryCells(parent);

        //EnemyManager.SetPathCell(pathGenerator.GenerateRoute());
        List<Vector2Int> cellPoints = pathGenerator.GenerateRoute();

        yield return new WaitForSeconds(2f);
        List<Vector3> path = new List<Vector3>();
        foreach (Vector2Int point in cellPoints)
        {
            path.Add(new Vector3(point.x, 1f, point.y));
        }
        loadedEnemyPath = path;
        SetUpEnemies(path);
    }
    private void SetUpEnemies(List<Vector3> path)
    {
        loadedEnemyPath = path; // For saving a level
        WaveManager.EnemySpawner.Path = path;
        WaveManager.enabled = true;
    }

    private void LayPathCells(List<Vector2Int> pathCells, Transform parent)
    {
        foreach(Vector2Int pathCell in pathCells)
        {
            int neighborValue = pathGenerator.GetCellNeighborValue(pathCell.x, pathCell.y);

            GameObject pathTile = pathCellObjects[neighborValue].cellPrefab;

            GameObject pathTileCell = Instantiate(pathTile, new Vector3(pathCell.x, 0f, pathCell.y), Quaternion.identity);
            pathTileCell.transform.parent = parent;
            pathTileCell.transform.Rotate(0f, pathCellObjects[neighborValue].yRotation, 0f);
            pathTileCell.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Path");
            pathTileCell.transform.GetChild(0).gameObject.tag = "Environment";
        }
    }

    private void LaySceneryCells(Transform parent)
    {
        int emptyCellWeight = 100;
        int filledCellWeight = 2;

        Dictionary<GridCellScriptableObject, int> weightTable = new();
        weightTable.Add(emptyCell, emptyCellWeight);
        foreach (GridCellScriptableObject cell in sceneryCellObjects)
            weightTable.Add(cell, filledCellWeight);
        
        for (int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if(pathGenerator.CellIsEmpty(x, y) && !pathGenerator.CellIsHazard(x, y))
                {
                    GridCellScriptableObject cell = GetWeightedItem(weightTable);
                    GameObject sceneryTileCell = Instantiate(cell.cellPrefab, new Vector3(x, 0f, y), GetCellRandomRotation());
                    sceneryTileCell.transform.parent = parent;
                    sceneryTileCell.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Environment");
                    sceneryTileCell.transform.GetChild(0).gameObject.tag = "Environment";
                    //yield return null;
                }
            }
        }
    }
    
    private GridCellScriptableObject GetWeightedItem(Dictionary<GridCellScriptableObject, int> weightTable)
    {
        int[] weights = weightTable.Values.ToArray();
        int randomWeight = Random.Range(0, weights.Sum());

        for(int i = 0; i < weights.Length; i++)
        {
            randomWeight -= weights[i];
            if(randomWeight < 0)
            {
                return weightTable.ElementAt(i).Key;
            }
        }

        return null;
    }
    private Quaternion GetCellRandomRotation()
    {
        int rotIndex = Random.Range(0, 4);
        Vector3 rotationVector = Vector3.zero;
        switch(rotIndex)
        {
            case 0:
                rotationVector.x = 0f;
                break;
            case 1:
                rotationVector.x = 90f;
                break;
            case 2:
                rotationVector.x = -90f;
                break;
            case 3:
                rotationVector.x = 180f;
                break;
        }
        return Quaternion.LookRotation(rotationVector);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        foreach (var point in EvauluateGridPoints())
        {
            Gizmos.DrawWireCube(point, new Vector3(1, 0, 1));
        }
    }

    IEnumerable<Vector3> EvauluateGridPoints()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for(int z = 0; z < gridHeight; z++)
            {
                yield return new Vector3(x, 0f, z);
            }
        }
    }

}

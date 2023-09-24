using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 8;

    [Tooltip("The lower the Min Path Length, the less Variation.")]
    public int minPathLength = 15;
    [Tooltip("The Max Path Length should be at least double the Grid Width, otherwise you might crash the application.")]
    public int maxPathLength = 40;

    /// <Summary> Enemy Manager relays the information from the grid and gives it to the enemies</Summary>
    private WaveManager WaveManager;

    /// <summary> Grid Cells, set in inspector, used to place paths based on the path grid</summary>
    public GridCellScriptableObject[] pathCellObjects;
    /// <summary> Grid Cells, set in inspector, fills empty space that is not used by the path grid </summary>
    public GridCellScriptableObject[] sceneryCellObjects;

    private PathGenerator pathGenerator;
    public GameObject loadedPath = null;
    public List<Vector3> loadedEnemyPath = new List<Vector3>();

    [SerializeField] private GameObject mapBoundPrefab;
    private void Awake()
    {
        Camera.main.transform.position = Vector3.zero + new Vector3(gridWidth / 2, (gridHeight * 2) / 4, -gridHeight / 3);
        // If the seed script is there, and the seed script's PickRandomSeed value is false:
        // Set the values of the grid/path to those saved in the MapVariables class.
        Seed seedScript = gameObject.GetComponent<Seed>();
        if(seedScript == null)
            return;
        if(seedScript.pickRandomSeed)
            return;

        Camera.main.transform.position = Vector3.zero + new Vector3(gridWidth / 2, (gridHeight * 2) / 4, -gridHeight / 3);
    }
    private void Start()
    {
        pathGenerator = new PathGenerator(gridWidth, gridHeight);
        WaveManager = GetComponent<WaveManager>();
        
        if(loadedPath == null)
            Generate();
        else
        {
            Instantiate(loadedPath);
            SetUpEnemies(loadedEnemyPath);
        }
        // Set up the tower placement bounds
        GameObject mapBoundary = Instantiate(mapBoundPrefab);
        mapBoundary.transform.position = new Vector3(0, 0, gridHeight/2);
        mapBoundary.transform.localScale = new Vector3(mapBoundary.transform.localScale.x, mapBoundary.transform.localScale.y, gridHeight*2);

        mapBoundary = Instantiate(mapBoundPrefab);
        mapBoundary.transform.position = new Vector3(gridWidth/2, 0, gridHeight);
        mapBoundary.transform.localScale = new Vector3(gridWidth*2, mapBoundary.transform.localScale.y, mapBoundary.transform.localScale.z);

        mapBoundary = Instantiate(mapBoundPrefab);
        mapBoundary.transform.position = new Vector3(gridWidth, 0, gridHeight/2);
        mapBoundary.transform.localScale = new Vector3(mapBoundary.transform.localScale.x, mapBoundary.transform.localScale.y, gridHeight*2);
    }

    /// <summary>
    /// Starts the generation and building of the path
    /// </summary>
    /// <param name="check"></param>
    private void Generate()
    {
        List<Vector2Int> pathCells = pathGenerator.GeneratePath();
        int pathSize = pathCells.Count;
        while (pathGenerator.GenerateCrossroads())
            pathSize = pathCells.Count;

        while (pathSize < minPathLength || pathSize > maxPathLength)
        {
            pathCells = pathGenerator.GeneratePath();
            while (pathGenerator.GenerateCrossroads())
                pathSize = pathCells.Count;
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
            pathTileCell.transform.GetChild(0).gameObject.tag = "Enviornment";
            //yield return null;
        }
        //yield return null;
    }

    private void LaySceneryCells(Transform parent)
    {
        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if(pathGenerator.CellIsEmpty(x, y))
                {
                    int randomCellIndex = Random.Range(0, sceneryCellObjects.Length);
                    GameObject sceneryTileCell = Instantiate(sceneryCellObjects[randomCellIndex].cellPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                    sceneryTileCell.transform.parent = parent;
                    sceneryTileCell.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Enviornment");
                    sceneryTileCell.transform.GetChild(0).gameObject.tag = "Enviornment";
                    //yield return null;
                }
            }
        }
    }

}

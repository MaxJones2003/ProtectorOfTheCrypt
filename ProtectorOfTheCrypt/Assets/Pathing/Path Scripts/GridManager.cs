using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 8;
    [SerializeField] private Transform hazards;
    [SerializeField] private GameObject hazardPrefab;

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

    // This should generate hazards at run time
    public void GenerateRandomPath(int GridWith, int GridHeight, int MinPathLength, int MaxPathLength, GameObject cameraPosition, int hazardGroupsToSpawn)
    {
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        
        gridWidth = GridWith;
        gridHeight = GridHeight;
        minPathLength = MinPathLength;
        maxPathLength = MaxPathLength;

        //Generate Hazards

        GenerateHazards(hazardGroupsToSpawn);

        pathGenerator = new PathGenerator(gridWidth, gridHeight, hazards);

        WaveManager = GetComponent<WaveManager>();

        hazards.gameObject.SetActive(true);
        Generate();

        GameObject centerPos = new GameObject();
        centerPos.transform.position = new Vector3(gridWidth / 2, 0, gridHeight / 2);

       
        virtualCamera.Follow = cameraPosition.transform;
        virtualCamera.LookAt = centerPos.transform;
        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.zero;

        StartCoroutine(SetupCameraPosition(cameraPosition));
    }
    private GameObject startPathPoint, endPathPoint;
    [SerializeField] private Transform camera;
    private IEnumerator SetupCameraPosition(GameObject cameraParent)
    {
        Renderer startRenderer = startPathPoint.GetComponentInChildren<Renderer>();
        Renderer endRenderer = endPathPoint.GetComponentInChildren<Renderer>();
        int iterations = 0;
        // while the camera is moving to the cameraParents initial position, wait
        while(Vector3.Distance(camera.position, cameraParent.transform.position) > 1f)
        {
            yield return new WaitForSeconds(0.1f);
        }


        while((!startRenderer.isVisible && !endRenderer.isVisible))
        {
            //if(iterations > 1000) break;
            cameraParent.transform.position += new Vector3(0f, 1f, -1f);
            iterations++;
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void GenerateHazards(int hazardGroupsToSpawn)
    {
        if(hazards == null)
        {
            Transform hazardParent = GameObject.FindWithTag("HazardParent").transform;
            if(hazardParent == null)
            {
                hazardParent = new GameObject().transform;
                hazardParent.name = "HazardParent";
                hazardParent.tag = "HazardParent";
            }
            hazards = hazardParent;
        }
        List<Vector3Int> hazardPositions = new();
        for(int i = 0; i < hazardGroupsToSpawn; i++)
        {
            int randSize = Random.Range(3, 10);
            Vector3Int randPos = new();
            int tries = 0;
            do 
            {
                int xPos = Random.Range(2, gridWidth-1);
                int zPos = Random.Range(2, gridHeight-1);
                randPos = new Vector3Int(xPos, 0, zPos);
                tries++;
                if(tries > 4) break;
            } while(!CheckGridValidity(randPos, hazardPositions) );
            CreateHazardGroup(randSize, randPos, ref hazardPositions);
        }

    }
    private void CreateHazardGroup(int size, Vector3Int pos, ref List<Vector3Int> hazardPositions)
    {
        List<Vector3Int> directions = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0),   // Center
            new Vector3Int(0, 0, 1),   // Up
            new Vector3Int(0, 0, -1),  // Down
            new Vector3Int(-1, 0, 0),  // Left
            new Vector3Int(1, 0, 0),   // Right
            new Vector3Int(1, 0, 1),   // Up Right
            new Vector3Int(-1, 0, 1),  // Up Left
            new Vector3Int(1, 0, -1),  // Down Right
            new Vector3Int(-1, 0, -1)    // Down Left
        };
        for(int i = 0; i < size; i++)
        {
            // Pick a random direction
            int index = Random.Range(0, directions.Count);
            Vector3Int newPos = pos + directions[index];
            directions.RemoveAt(index);

            Transform child = Instantiate(hazardPrefab, newPos, Quaternion.identity).transform;
            child.parent = hazards;
            hazardPositions.Add(newPos);
        }
    }
    private bool CheckGridValidity(Vector3Int position, List<Vector3Int> hazards)
    {
        return hazards.Contains(position);
    }

    // This is for in editor and doesn't need to generate random hazards at run time
    public void GenerateRandomPath()
    {
        pathGenerator = new PathGenerator(gridWidth, gridHeight, hazards);

        WaveManager = GetComponent<WaveManager>();
        
        Camera.main.transform.position = Vector3.zero + new Vector3(gridWidth / 2, (gridHeight * 2) / 2, -gridHeight / 3);
        Camera.main.transform.LookAt(new Vector3(gridWidth / 2, 0, gridHeight / 2 - 4));
        //Camera.main.transform.position = Vector3.zero + new Vector3(gridWidth / 2, (gridHeight * 2) / 2, -gridHeight / 3);
        hazards.gameObject.SetActive(true);
        Generate();
    }

    public void GenerateLoadedPath(int GridWith, int GridHeight, int MinPathLength, int MaxPathLength, GameObject LoadedPath, List<Vector3> EnemyPath)
    {
        WaveManager = GetComponent<WaveManager>();

        gridWidth = GridWith;
        gridHeight = GridHeight;
        minPathLength = MinPathLength;
        maxPathLength = MaxPathLength;
        loadedPath = LoadedPath;
        loadedEnemyPath = EnemyPath;
        Instantiate(loadedPath);
        SetUpEnemies(loadedEnemyPath);
        Camera.main.transform.position = Vector3.zero + new Vector3(gridWidth / 2, (gridHeight * 2) / 2, -gridHeight / 3);
        Camera.main.transform.LookAt(new Vector3(gridWidth / 2, 0, gridHeight / 2 - 4));

        hazards.gameObject.SetActive(false);
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
        parent.parent = transform;
        parent.SetAsFirstSibling();
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
        for(int i = 0; i < pathCells.Count; i++)
        {
            Vector2Int pathCell = pathCells[i];
            int neighborValue = pathGenerator.GetCellNeighborValue(pathCell.x, pathCell.y);

            GameObject pathTile = pathCellObjects[neighborValue].cellPrefab;

            GameObject pathTileCell = Instantiate(pathTile, new Vector3(pathCell.x, 0f, pathCell.y), Quaternion.identity);
            pathTileCell.transform.parent = parent;
            pathTileCell.transform.Rotate(0f, pathCellObjects[neighborValue].yRotation, 0f);
            pathTileCell.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Path");
            pathTileCell.transform.GetChild(0).gameObject.tag = "Environment";

            if(i == 0)
            {
                startPathPoint = pathTileCell;
            }
            else if(i == pathCells.Count - 1)
            {
                endPathPoint = pathTileCell;
            }
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
                    GameObject sceneryTileCell = Instantiate(cell.cellPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                    sceneryTileCell.transform.rotation = GetCellRandomRotation();
                    sceneryTileCell.transform.parent = parent;
                    //sceneryTileCell.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Environment");
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
        if(rotationVector == Vector3.zero) return Quaternion.identity;
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

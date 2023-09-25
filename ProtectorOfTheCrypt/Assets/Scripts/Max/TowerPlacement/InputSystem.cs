using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    [SerializeField] private List<TowerScriptableObject> PurchasableTowers;
    [SerializeField] private Grid grid;

    private TowerScriptableObject currentTowerScriptableObject = null;
    private GameObject currentTowerModel = null;

    [SerializeField] private Camera sceneCamaera;
    private Vector3 lastPosition;

    [SerializeField] private LayerMask placementLayerMask;
    [SerializeField] private LayerMask boundsLayerMask;
    [SerializeField] private LayerMask towerLayerMask;

    private bool towerCurrentlySelected = false;


    private PlayerInput playerInput;
    private InputAction standardClick;
    private InputAction placementClick;

    // This allows the script to change the layer of the gridspace where a tower was placed, preventing another tower from being placed on that gridspace
    private RaycastHit hit;

    private void Awake() 
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("StandardMode");
        standardClick = playerInput.actions["StandardMode/Click"];
        placementClick = playerInput.actions["TowerPlacementMode/Click"];

        standardClick.started += CheckForTower;
        placementClick.started += SetTowerDown;
    }
    private void Update() 
    {
        if(currentTowerModel != null)
        {
            Vector3 mousePosition = Vector3.zero;
            (mousePosition, hit) = GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            currentTowerModel.transform.position = grid.CellToWorld(gridPosition);
        }
    }

    #region Tower Placement
    /// <summary>
    /// Instantiates the Model of the selected tower. This allows the player to move the tower around the grid, before actually purchasing it.
    /// </summary>
    public void SelectTower(string nameOfTowerToSelect)
    {
        // Find the correct tower based on the string given
        TowerScriptableObject tower = PurchasableTowers.Find(t => t.Name == nameOfTowerToSelect);

        // If nothing was found: return and give an error
        if(tower == null)
        {
            Debug.LogError($"No TowerScriptableObject found for TowerName: {nameOfTowerToSelect}");
            return;
        }

        // Spawn the model
        TowerScriptableObject newTowerInstance = CreateNewTowerInstance(tower);
        currentTowerScriptableObject = newTowerInstance;
        currentTowerModel = currentTowerScriptableObject.SpawnModel(this);
        towerCurrentlySelected = true;

        // Change the action map to tower placement mode
        playerInput.SwitchCurrentActionMap("TowerPlacementMode");
    }

    public (Vector3, RaycastHit) GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamaera.nearClipPlane;
        Ray ray = sceneCamaera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.transform.position;
        }
        else if(Physics.Raycast(ray, out hit, 100, boundsLayerMask))
        {
            lastPosition = hit.point;
            lastPosition.y = 0;
        }
        return (lastPosition, hit);
    }

    private TowerScriptableObject CreateNewTowerInstance(TowerScriptableObject originalTower)
    {
        TowerScriptableObject newTowerInstance = Instantiate(originalTower);
        // Set any additional properties or configurations for the new tower instance if needed
        return newTowerInstance;
    }

    /// <summary>
    /// If the player has a selected tower, they may choose to cancel the selection, this destroys the currentTower and stops the IEnumerator responsible for updateing the tower position.
    /// </summary>
    public void CancelTowerPlacement()
    {
        Destroy(currentTowerModel);
        currentTowerModel = null;
        towerCurrentlySelected = false;

        // Return to standard mode action map
        playerInput.SwitchCurrentActionMap("StandardMode");
    }

    /// <summary>
    /// Once the player has decided where to place the tower, SetTowerDown runs the code that sets up the rest of the 
    /// </summary>
    public void SetTowerDown(InputAction.CallbackContext ctx)
    {
        if(!towerCurrentlySelected)
            return;
        currentTowerModel.AddComponent<ShootMonoBehaviour>().tower = currentTowerScriptableObject;
        currentTowerScriptableObject.Spawn();
        currentTowerModel = null;
        currentTowerScriptableObject = null;
        towerCurrentlySelected = false;
        hit.transform.gameObject.layer = LayerMask.NameToLayer("Default");

        // Return to standard mode action map
        playerInput.SwitchCurrentActionMap("StandardMode");

    }
    #endregion

    #region Tower Interaction
    public void CheckForTower(InputAction.CallbackContext ctx)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamaera.nearClipPlane;
        Ray ray = sceneCamaera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, towerLayerMask))
        {
            Debug.Log(hit.transform.name);
        }
    }
    #endregion
}

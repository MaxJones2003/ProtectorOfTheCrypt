using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class InputSystem : MonoBehaviour
{
    private GridManager gridManager;
    [SerializeField] private List<TowerScriptableObject> PurchasableTowers;
    [SerializeField] private Grid grid;

    private TowerScriptableObject currentTowerScriptableObject = null;
    private GameObject currentTowerModel = null;

    [SerializeField] private Camera sceneCamaera;
    private Vector3 lastPosition;
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private LayerMask placementLayerMask;
    [SerializeField] private LayerMask gridLayerMask;
    [SerializeField] private LayerMask towerLayerMask;
    [SerializeField] private LayerMask UILayerMask;


    [SerializeField] private GameObject UIButtons;
    [SerializeField] private GameObject towerSelectionUI;


    private bool towerCurrentlySelected = false;


    private PlayerInput playerInput;
    private InputAction standardClick;
    private InputAction placementClick;

    // This allows the script to change the layer of the gridspace where a tower was placed, preventing another tower from being placed on that gridspace
    private RaycastHit hit;

    private GameObject currentTowerForUpgrade;
    private Vector3Int gridPosition;
    private bool isIndicatorWhite = false;
    private bool canPlaceTowers = false;
    public bool isTowerPlacementUIActive = false;

    // Ref for InGameTowerSelection
    [SerializeField] IGTS_Buttons IGTS_UI;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("StandardMode");
        standardClick = playerInput.actions["StandardMode/Click"];
        placementClick = playerInput.actions["TowerPlacementMode/Click"];

        standardClick.started += CheckForTower;
        placementClick.started += SetTowerDown;

        sceneCamaera = Camera.main;

        placementIndicator = Instantiate(placementIndicator);
    }


    private void OnDisable()
    {
        standardClick.started -= CheckForTower;
        placementClick.started -= SetTowerDown;
    }

    private void Update()
    {
        if (GameManager.instance.isPaused)
        {
            return;
        } 
        if (currentTowerModel == null && !isTowerPlacementUIActive)
        {
            Vector3 mousePosition;
            LayerMask layer;
            (mousePosition, layer) = GetSelectedMapPosition();
            gridPosition = grid.WorldToCell(mousePosition);
            placementIndicator.transform.position = grid.CellToWorld(gridPosition);

            // Check if the layer has changed and set the color accordingly
            bool newIndicatorWhite = (layer & placementLayerMask) != 0;
            if (newIndicatorWhite != isIndicatorWhite)
            {
                isIndicatorWhite = newIndicatorWhite;
                CheckLayerAndSetColor();
            }
        }
    }

    #region Tower Placement
    /// <summary>
    /// Instantiates the Model of the selected tower. This allows the player to move the tower around the grid, before actually purchasing it.
    /// </summary>
    public void SelectTower(string nameOfTowerToSelect)
    {
        if (GameManager.instance.isPaused) return;
        // Find the correct tower based on the string given
        TowerScriptableObject tower = PurchasableTowers.Find(t => t.Name == nameOfTowerToSelect);

        // If nothing was found: return and give an error
        if (tower == null)
        {
            Debug.LogError($"No TowerScriptableObject found for TowerName: {nameOfTowerToSelect}");
            return;
        }

        // Spawn the model

        currentTowerScriptableObject = tower;
        currentTowerModel = currentTowerScriptableObject.SpawnModel(this, placementIndicator.transform.position);
        // Make the position below the tower un-placable.
        ChangeGridLayer();
        towerCurrentlySelected = true;

        // Change the action map to tower placement mode
        playerInput.SwitchCurrentActionMap("TowerPlacementMode");

        if (!towerCurrentlySelected)
            return;
        currentTowerScriptableObject.Spawn();
        currentTowerModel.AddComponent<ShootMonoBehaviour>().Activate(currentTowerScriptableObject);
        currentTowerModel = null;
        currentTowerScriptableObject = null;
        towerCurrentlySelected = false;


        // Return to standard mode action map
        playerInput.SwitchCurrentActionMap("StandardMode");
    }
    private void ChangeGridLayer()
    {
        Ray ray = new Ray(placementIndicator.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.gameObject.layer != LayerMask.NameToLayer("Environment"))
            {
                Debug.LogError("The Raycast did not hit a valid grid position.");
                return;
            }
            hitInfo.collider.gameObject.layer = LayerMask.NameToLayer("Hazard");
        }
    }

    public (Vector3, LayerMask) GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamaera.nearClipPlane;
        Ray ray = sceneCamaera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        LayerMask layer = LayerMask.NameToLayer("default");
        if (Physics.Raycast(ray, out hit, 100, gridLayerMask))
        {
            lastPosition = hit.collider.transform.position;
            lastPosition.y = 1f;
            layer = 1 << hit.transform.gameObject.layer;
        }

        return (lastPosition, layer);
    }
    private void CheckLayerAndSetColor()
    {
        if (isIndicatorWhite)
        {
            // Set the material color to white
            canPlaceTowers = true;
            placementIndicator.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        else
        {
            canPlaceTowers = false;
            // Set the material color to red
            placementIndicator.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    /// <summary>
    /// Once the player has decided where to place the tower, SetTowerDown runs the code that sets up the rest of the 
    /// </summary>
    public void SetTowerDown(InputAction.CallbackContext ctx)
    {
        if (!towerCurrentlySelected)
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
        if (GameManager.instance.isPaused) return;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamaera.nearClipPlane;
        Ray ray = sceneCamaera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, 1000, UILayerMask)) return;
        else if (Physics.Raycast(ray, out hit, 100, towerLayerMask))
        {
            TowerPlacementMode(false);
            // Deactivate the previous upgrade ui
            if (currentTowerForUpgrade != null)
            {
                currentTowerForUpgrade.GetComponent<TowerUpgradeHandler>().ActivateUI(false);
            }
            // Activate the new upgrade ui
            currentTowerForUpgrade = hit.transform.parent.gameObject;
            currentTowerForUpgrade.GetComponent<TowerUpgradeHandler>().ActivateUI(true);

            UIButtons.SetActive(true);
        }
        else if (canPlaceTowers && currentTowerForUpgrade == null)
        {
            // Activate IGTS ui
            IGTS_UI.ActivateUI(placementIndicator.transform.position);
            TowerPlacementMode(true);
            //SelectTower("ExplosiveTower");
        }
        else
        {
            TowerPlacementMode(false);
            if (currentTowerForUpgrade == null)
                return;
            currentTowerForUpgrade.GetComponent<TowerUpgradeHandler>().ActivateUI(false);
            currentTowerForUpgrade = null;
        }
    }

    public void TowerPlacementMode(bool isActive)
    {
        isTowerPlacementUIActive = isActive;

    }


    #endregion
}


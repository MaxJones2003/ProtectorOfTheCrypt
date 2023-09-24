using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private List<TowerScriptableObject> PurchasableTowers;
    [SerializeField] private PlacementInputManger placementInputManger;
    [SerializeField] private Grid grid;

    private TowerScriptableObject currentTowerScriptableObject = null;
    private GameObject currentTowerModel = null;

    private bool towerCurrentlySelected = false;

    // This allows the script to change the layer of the gridspace where a tower was placed, preventing another tower from being placed on that gridspace
    private RaycastHit hit;

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && towerCurrentlySelected == true)
        {
            SetTowerDown();
        }

        if(currentTowerModel != null)
        {
            Vector3 mousePosition = Vector3.zero;
            (mousePosition, hit) = placementInputManger.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            currentTowerModel.transform.position = grid.CellToWorld(gridPosition);
        }
    }


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
    }

    /// <summary>
    /// Once the player has decided where to place the tower, SetTowerDown runs the code that sets up the rest of the 
    /// </summary>
    public void SetTowerDown()
    {
        currentTowerModel.AddComponent<ShootMonoBehaviour>().tower = currentTowerScriptableObject;
        currentTowerScriptableObject.Spawn();
        currentTowerModel = null;
        currentTowerScriptableObject = null;
        towerCurrentlySelected = false;
        hit.transform.gameObject.layer = LayerMask.NameToLayer("Default");
    }
}

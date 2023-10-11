using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGTS_Buttons : MonoBehaviour
{
    public GameObject IGTS;
    [SerializeField] InputSystem inputRef;
    public void ActivateUI(Vector3 position)
    {
        position.y = 5;
        IGTS.transform.position = position;
        IGTS.SetActive(true);
    }
    public void SpawnArchers()
    {
        if(StoreManager.Instance.CannotBuy(StoreManager.Instance.archerCost))
        {
            // Maybe add some code here to make the button flash red
            return;
        } 

        StoreManager.Instance.Purchase(StoreManager.Instance.archerCost);
        inputRef.SelectTower("ExampleTower");
        IGTS.SetActive(false);
        inputRef.TowerPlacementMode(false);
    }

    public void SpawnBombers()
    {
        if(StoreManager.Instance.CannotBuy(StoreManager.Instance.bomberCost))
        {
            // Maybe add some code here to make the button flash red
            return;
        } 

        StoreManager.Instance.Purchase(StoreManager.Instance.bomberCost);
        inputRef.SelectTower("ExplosiveTower");
        IGTS.SetActive(false);
        inputRef.TowerPlacementMode(false);
    }

    public void CancelButton()
    {
        IGTS.SetActive(false);
        inputRef.TowerPlacementMode(false);
    }
}

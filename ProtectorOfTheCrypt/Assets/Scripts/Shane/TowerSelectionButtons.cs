using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TowerSelectionButtons : MonoBehaviour
{
    public UIButtons UIRef;                                 // Reference to the UIButtons (pause and Tower Box)
    [SerializeField] private InputSystem PlaceTowerRef;                   // Reference to Max's Tower Placement System

    // Update is called once per frame
    void Update()
    {

    }

    public void Tower1(int price = 0)
    {
        if(GameManager.instance.RemoveMoney(price))
            PlaceTowerRef.SelectTower("ExampleTower");          // Change the Name in ""s to match Archers
    }

    public void Tower2(int price = 0)
    {
        if (GameManager.instance.RemoveMoney(price)) 
            PlaceTowerRef.SelectTower("ExplosiveTower");                // Change the Name in ""s to match Bombers
    }

    // Cancel() is to get rid of the Tower Selection Menu
    public void Cancel()
    {
        UIRef.TowerSelection.SetActive(false);
        UIRef.UIButton.SetActive(true);
    }
}

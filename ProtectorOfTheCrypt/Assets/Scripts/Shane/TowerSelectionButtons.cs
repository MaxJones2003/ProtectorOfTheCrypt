using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSelectionButtons : MonoBehaviour
{
    public UIButtons UIRef;                                 // Reference to the UIButtons (pause and Tower Box)
    private PlacementSystem PlaceTowerRef;                   // Reference to Max's Tower Placement System

    // Update is called once per frame
    void Update()
    {

    }

    public void Tower1()
    {
        PlaceTowerRef.SelectTower("ExampleTower");          // Change the Name in ""s to match Archers
    }

    public void Tower2()
    {
        PlaceTowerRef.SelectTower("Bomber");                // Change the Name in ""s to match Bombers
    }

    // Cancel() is to get rid of the Tower Selection Menu
    public void Cancel()
    {
        UIRef.TowerSelection.SetActive(false);
        UIRef.UIButton.SetActive(true);
    }
}

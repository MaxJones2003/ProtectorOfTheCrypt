using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGTS_Buttons : MonoBehaviour
{
    public GameObject IGTS;
    InputSystem inputRef;
    public void SpawnArchers()
    {
        inputRef.SelectTower("ExampleTower");
        IGTS.SetActive(false);
    }

    public void SpawnBombers()
    {
        inputRef.SelectTower("ExplosiveTower");
        IGTS.SetActive(false);
    }

    public void CancelButton()
    {
        IGTS.SetActive(false);
    }
}

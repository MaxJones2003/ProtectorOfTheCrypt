using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGTS_Buttons : MonoBehaviour
{
    public GameObject IGTS;
    [SerializeField] InputSystem inputRef;
    public void ActivateUI(Vector3 position)
    {
        IGTS.transform.position = position;
        IGTS.SetActive(true);
    }
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

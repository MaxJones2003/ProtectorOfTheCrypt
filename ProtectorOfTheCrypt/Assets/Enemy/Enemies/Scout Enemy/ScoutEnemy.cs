using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutEnemy : MonoBehaviour
{
    void OnDestroy()
    {
        GameManager.instance.gameStarted = true;
    }
}

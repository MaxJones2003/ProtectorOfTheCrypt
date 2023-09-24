using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ShootMonoBehaviour : MonoBehaviour
{
    public TowerScriptableObject tower;
    private bool paused;


    #region Upgrades
    public float damageModifierUpgrade = 1f;
    #endregion
    public void Awake()
    {
        GameManager.instance.OnGamePaused += UpdateGamePaused;
    }
    private void UpdateGamePaused(bool isPaused)
    {
        paused = isPaused;
    }
    private void OnDestroy()
    {
        GameManager.instance.OnGamePaused -= UpdateGamePaused;
    }
    private void Update()
    {
        if(paused)
            return;
        
        tower.Shoot(damageModifierUpgrade);
    }
}

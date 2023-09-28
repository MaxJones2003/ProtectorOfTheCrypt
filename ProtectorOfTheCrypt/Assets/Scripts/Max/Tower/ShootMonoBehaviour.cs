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
    public float shootTimeUpgrade = 0f;
    #endregion
    public void OnEnable()
    {
        GameManager.instance.OnGamePaused += UpdateGamePaused;
    }
    private void UpdateGamePaused(bool isPaused)
    {
        paused = isPaused;
    }
    private void OnDisable()
    {
        GameManager.instance.OnGamePaused -= UpdateGamePaused;
    }
    private void Update()
    {
        if(paused)
            return;
        
        tower.Shoot(damageModifierUpgrade, shootTimeUpgrade);
    }
}

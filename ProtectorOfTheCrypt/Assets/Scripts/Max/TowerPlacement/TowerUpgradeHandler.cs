using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgradeHandler : MonoBehaviour
{
    [SerializeField] private GameObject upgradeUI;

    private void Start()
    {
        upgradeUI.SetActive(false);
        upgradeUI.transform.GetChild(0).gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void ActivateUI(bool activate)
    {
        upgradeUI.SetActive(activate);
    }

    public void UpgradeDamage()
    {
        if (GameManager.instance.RemoveMoney(25))
        {
            gameObject.GetComponent<ShootMonoBehaviour>().damageModifierUpgrade += 0.2f;
            gameObject.GetComponent<ShootMonoBehaviour>().shootTimeUpgrade += 0.1f;
            gameObject.GetComponent<ShootMonoBehaviour>().shootTimeUpgrade = Mathf.Clamp(gameObject.GetComponent<ShootMonoBehaviour>().shootTimeUpgrade, 0f, 1f);
        }
        
    }

    public void SellTower()
    {
        GameManager.instance.RemoveMoney(-5);
        Destroy(gameObject);
    }
}

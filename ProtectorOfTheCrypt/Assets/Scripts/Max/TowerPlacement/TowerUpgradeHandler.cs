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
        Debug.Log("clicked upgrade");
        gameObject.GetComponent<ShootMonoBehaviour>().damageModifierUpgrade += 0.2f;
    }
}

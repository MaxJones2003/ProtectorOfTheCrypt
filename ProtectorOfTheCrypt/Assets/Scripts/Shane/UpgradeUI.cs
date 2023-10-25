using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    public GameObject UpUI;
    public TowerUpgradeHandler TowerUpRef;
    public ParticleSystem upgradePS;

    // Appear() is to make the Upgrade Ui Appear. Call this function when the player WANTS to see the upgrade UI
    public void Appear()
    {
        UpUI.SetActive(false);
    }

    // Hide() is to Hide the Upgrade Ui. Call this function when the player does NOT want to see the upgrade UI
    public void Hide()
    {
        UpUI.SetActive(false);
    }

    // Start of Button Functions
    public void DamageButton()
    {
        // Call UpgradeDamageAdd(upgrade amount, cost);
        TowerUpRef.UpgradeDamageAdd(2, 25);
        upgradePS.Play();
    }

    public void RangeButton()
    {
        // Call UpgradeRangeAdd(upgrade amount, cost);
        TowerUpRef.UpgradeRangeAdd(2, 15);
        upgradePS.Play();
    }

    public void FireRateButton()
    {
        // Call UpgradeFireRateAdd(upgrade amount, cost);
        TowerUpRef.UpgradeFireRateSubtract(2, 20);
        upgradePS.Play();
    }
}

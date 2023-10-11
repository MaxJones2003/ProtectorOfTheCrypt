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

    public void UpgradeDamageAdd(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        AddDamageModifier damageModifier = new()
        {
            Amount = upgrade,
            AttributeName = "DamageConfig/DamageCurve"
        };
        damageModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
    }
    public void UpgradeDamageMultiply(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        MultiplyDamageModifier damageModifier = new()
        {
            Amount = upgrade,
            AttributeName = "DamageConfig/DamageCurve"
        };
        damageModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
    }
    public void UpgradeAOEDamageAdd(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        AddAOEDamageModifier damageModifier = new()
        {
            Amount = upgrade,
            AttributeName = "DamageConfig/AOEDamage"
        };
        damageModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
        gameObject.GetComponent<ShootMonoBehaviour>().SetExplosiveDamage();
    }
    public void UpgradeAOERangeAdd(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        AddRangeModifier rangeModifier = new()
        {
            Amount = upgrade,
            AttributeName = "DamageConfig/AOERange"
        };
        rangeModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
        gameObject.GetComponent<ShootMonoBehaviour>().SetExplosiveDamage();
    }
    public void UpgradeRangeAdd(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        AddRangeModifier rangeModifier = new()
        {
            Amount = upgrade,
            AttributeName = "ProjectileConfig/Range"
        };
        rangeModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
    }
    public void UpgradeFireRateSubtract(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        SubtractFireRateModifier fireRateModifier = new()
        {
            Amount = upgrade,
            AttributeName = "ProjectileConfig/FireRate"
        };
        fireRateModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
    }
    public void SellTower()
    {
        int sellPrice = -5;
        StoreManager.Instance.Purchase(sellPrice);
        Destroy(gameObject);
    }
}

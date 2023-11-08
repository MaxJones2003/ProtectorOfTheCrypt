using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgradeHandler : MonoBehaviour
{
    [SerializeField] private GameObject upgradeUITier2;
    [SerializeField] private GameObject upgradeUITier3;
    [SerializeField] private GameObject upgradeUITier2Model;
    [SerializeField] private GameObject upgradeUITier3Model;

    private bool hasUpgradedOnce;
    private bool hasUpgradedTwice;

    [SerializeField] private ParticleSystem upgradePS;

    private void Start()
    {
        upgradeUITier2.SetActive(false);
        upgradeUITier2.transform.GetChild(0).gameObject.GetComponent<Canvas>().worldCamera = Camera.main;

        upgradeUITier3.SetActive(false);
        upgradeUITier3.transform.GetChild(0).gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void ActivateUI(bool activate)
    {
        if (!hasUpgradedOnce)
            upgradeUITier2.SetActive(activate);
        if(hasUpgradedOnce && !hasUpgradedTwice)
            upgradeUITier3.SetActive(activate);
    }

    public void UpgradeDamageAdd(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        upgradePS.Play();
        AddDamageModifier damageModifier = new()
        {
            Amount = upgrade,
            AttributeName = "DamageConfig/DamageCurve"
        };

        damageModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
        ApplyUpgradeFlag();
        ApplyUpgradedModel();
    }

    public void UpgradeDamageMultiply(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        upgradePS.Play();
        MultiplyDamageModifier damageModifier = new()
        {
            Amount = upgrade,
            AttributeName = "DamageConfig/DamageCurve"
        };
        damageModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
        ApplyUpgradeFlag();
    }
    public void UpgradeAOEDamageAdd(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        upgradePS.Play();
        AddAOEDamageModifier damageModifier = new()
        {
            Amount = upgrade,
            AttributeName = "DamageConfig/AOEDamage"
        };
        damageModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
        gameObject.GetComponent<ShootMonoBehaviour>().SetExplosiveDamage();
        ApplyUpgradeFlag();
    }
    public void UpgradeAOERangeAdd(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        upgradePS.Play();
        AddRangeModifier rangeModifier = new()
        {
            Amount = upgrade,
            AttributeName = "DamageConfig/AOERange"
        };
        rangeModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
        gameObject.GetComponent<ShootMonoBehaviour>().SetExplosiveDamage();
        ApplyUpgradeFlag();
    }
    public void UpgradeRangeAdd(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        upgradePS.Play();
        AddRangeModifier rangeModifier = new()
        {
            Amount = upgrade,
            AttributeName = "ProjectileConfig/Range"
        };
        rangeModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
        ApplyUpgradeFlag();
    }
    public void UpgradeFireRateSubtract(float upgrade, int cost)
    {
        if (StoreManager.Instance.CannotBuy(cost)) return;
        StoreManager.Instance.Purchase(cost);
        upgradePS.Play();
        SubtractFireRateModifier fireRateModifier = new()
        {
            Amount = upgrade,
            AttributeName = "ProjectileConfig/FireRate"
        };
        fireRateModifier.Apply(gameObject.GetComponent<ShootMonoBehaviour>().tower);
        ApplyUpgradeFlag();
    }
    public void SellTower()
    {
        int sellPrice = -5;
        StoreManager.Instance.Purchase(sellPrice);
        Destroy(gameObject);
    }

    private void ApplyUpgradeFlag()
    {
        if (!hasUpgradedOnce)
            hasUpgradedOnce = true;
        if (hasUpgradedOnce && !hasUpgradedTwice)
            hasUpgradedTwice = true;
    }

    private void ApplyUpgradedModel()
    {

    }

}

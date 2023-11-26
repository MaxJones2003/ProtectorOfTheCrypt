using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;
    [Header("Tower Prices")]
    public int archerCost;
    public int bomberCost;
    public int slowCost = 30;
    //public int wizardCost;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public bool CannotBuy(int cost)
    {
        if(cost <= GameManager.instance.Money)
            return false;
        return true;
    }

    public void Purchase(int cost)
    {
        GameManager.instance.RemoveMoney(cost);
    }
}

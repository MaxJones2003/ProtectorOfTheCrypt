using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public void Start()
    {
        GameManager.instance.OnMoneyChanged += UpdateCounter;
        text.text = "Money: " + GameManager.instance.Money;
    }

    public void OnDestroy()
    {
        GameManager.instance.OnMoneyChanged -= UpdateCounter;
    }

    public void UpdateCounter()
    {
        text.text = "Money: " + GameManager.instance.Money;
    }
}

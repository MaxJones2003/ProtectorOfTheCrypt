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
        text.text = GameManager.instance.Money.ToString();
    }

    public void OnDestroy()
    {
        GameManager.instance.OnMoneyChanged -= UpdateCounter;
    }

    public void UpdateCounter()
    {
        text.text = GameManager.instance.Money.ToString();
    }
}

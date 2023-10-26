using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public void OnEnable()
    {
        GameManager.OnMoneyChanged += UpdateCounter;
    }

    public void Start()
    {
        text.text = GameManager.instance.Money.ToString();
    }

    public void OnDestroy()
    {
        GameManager.OnMoneyChanged -= UpdateCounter;
    }

    public void UpdateCounter()
    {
        text.text = GameManager.instance.Money.ToString();
    }
}

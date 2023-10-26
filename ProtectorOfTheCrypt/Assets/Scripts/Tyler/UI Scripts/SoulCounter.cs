using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SoulCounter : MonoBehaviour
{
	public TextMeshProUGUI text;

    public void Start()
    {
        text.text = GameManager.instance.Souls.ToString();
    }
    public void OnEnable()
	{
        GameManager.OnSoulsChanged += UpdateCounter;
    }

    public void OnDisable()
    {
        GameManager.OnSoulsChanged -= UpdateCounter;
    }

    public void UpdateCounter()
    {
		text.text = GameManager.instance.Souls.ToString();
    }
}

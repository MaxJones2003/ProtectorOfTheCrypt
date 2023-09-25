using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SoulCounter : MonoBehaviour
{
	public TextMeshProUGUI text;
	public void Start()
	{
		GameManager.instance.OnSoulsChanged += UpdateCounter;
        text.text = "Souls Left: " + GameManager.instance.Souls;
    }

    public void OnDestroy()
    {
        GameManager.instance.OnSoulsChanged -= UpdateCounter;
    }

    public void UpdateCounter()
    {
		text.text = "Souls Left: " + GameManager.instance.Souls;
    }
}

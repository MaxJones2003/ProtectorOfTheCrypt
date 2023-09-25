using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    public GameObject UpUI;

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
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour
{
    [Header("Endless Mode Settings")]
    public MapSizeSettings easyMap;
    public MapSizeSettings standardMap;
    public MapSizeSettings hardMap;
    public EnemyDifficultySettings easyEnemy;
    public EnemyDifficultySettings standardEnemy;
    public EnemyDifficultySettings hardEnemy;

    private MapSizeSettings currentMapValue;
    private EnemyDifficultySettings currentEnemyValue;
    private string seed;

    [Header("Keypad")]
    public TMP_InputField charHolder;
    public GameObject b1;
    public GameObject b2;
    public GameObject b3;
    public GameObject b4;
    public GameObject b5;
    public GameObject b6;
    public GameObject b7;
    public GameObject b8;
    public GameObject b9;
    public GameObject b0;
    public GameObject Clear;
    public GameObject Enter;
    int keypadNumber;

    [Header("Dropdowns")]
    [SerializeField] private TMP_Dropdown mapDropdown;
    int mapValHolder;
    [SerializeField] private TMP_Dropdown enemyDropdown;
    int enemyValHolder;

    void Start()
    {
        charHolder.characterLimit = 16;
        mapValHolder = 0;
        enemyValHolder = 0;
        MapDiffChanged();
        EnemyDiffChanged();
        seed = Seed.Instance.CreateRandomSeed(charHolder.characterLimit, true);
        charHolder.text = seed;
    }

    public void KeypadValueChanged()
    {
        Debug.Log(charHolder.text);
    }

    public void MapDiffChanged()
    {
        mapValHolder = mapDropdown.value;

        // 0 = easy, 1 = standard, 2 = hard
        if (mapValHolder == 0)
        {
            currentMapValue = easyMap;
        }
        else if (mapValHolder == 1)
        {
            currentMapValue = standardMap;
        }
        else if (mapValHolder == 2)
        {
            currentMapValue = hardMap;
        }
    }

    // 0 = easy, 1 = standard, 2 = hard
    public void EnemyDiffChanged()
    {
        enemyValHolder = enemyDropdown.value;
        
        // 0 = easy, 1 = standard, 2 = hard
        if (enemyValHolder == 0)
        {
            currentEnemyValue = easyEnemy;
        }
        else if (enemyValHolder == 1)
        {
            currentEnemyValue = standardEnemy;
        }
        else if (enemyValHolder == 2)
        {
            currentEnemyValue = hardEnemy;
        }
    }

    // Finish Button
    public void Ready()
    {
        EndlessModeSettings settings = new(seed, currentMapValue, currentEnemyValue);

        EndlessMode mode = GameManager.instance.GameMode as EndlessMode;
        Debug.Log(settings);
        mode.ReadyToLoadMap(settings);
    }

    #region Keypad

    public void B1()
    {
        keypadNumber = 1;
        UpdateString(keypadNumber);
    }

    public void B2()
    {
        keypadNumber = 2;
        UpdateString(keypadNumber);
    }

    public void B3()
    {
        keypadNumber = 3;
        UpdateString(keypadNumber);
    }

    public void B4()
    {
        keypadNumber = 4;
        UpdateString(keypadNumber);
    }

    public void B5()
    {
        keypadNumber = 5;
        UpdateString(keypadNumber);
    }

    public void B6()
    {
        keypadNumber = 6;
        UpdateString(keypadNumber);
    }

    public void B7()
    {
        keypadNumber = 7;
        UpdateString(keypadNumber);
    }

    public void B8()
    {
        keypadNumber = 8;
        UpdateString(keypadNumber);
    }

    public void B9()
    {
        keypadNumber = 9;
        UpdateString(keypadNumber);
    }

    public void B0()
    {
        keypadNumber = 0;
        UpdateString(keypadNumber);
    }

    public void ClearButton()
    {
        charHolder.text = null;
    }

    public void SaveButton()
    {
        seed = charHolder.text;
    }

    public void UpdateString(int i)
    {
        if (charHolder.text.Length == charHolder.characterLimit)
        {
            return;
        }

        charHolder.text = charHolder.text + i.ToString();
        SaveButton();
    }

    #endregion
}
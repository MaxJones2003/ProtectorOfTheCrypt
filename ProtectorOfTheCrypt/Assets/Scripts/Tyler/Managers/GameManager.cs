using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<bool> OnGamePaused;
    public static event Action OnSoulsChanged;
    public static event Action OnMoneyChanged;

    public static GameManager instance;

    public GameMode GameMode;
    public bool isPaused;
    public bool gameStarted;
    [SerializeField] private int souls = 100;
    [SerializeField] private int money = 50;
    public int Souls { get { return souls; } private set { souls = value; } }
    public int Money { get { return money; } private set { money = value; } }

    public void Awake()
    {
        instance = this;
        GameMode = gameObject.GetComponent<GameMode>();
    }

    public void RemoveSouls(int LostSouls)
    {
        Souls -= LostSouls;
        OnSoulsChanged?.Invoke();
        if ((GameMode is StoryMode || GameMode is EndlessMode) && GameMode.CheckGameLost())
        {
            GameMode.OnGameLost();
        }
            
    }

    // Pass a negative number to give money to the player.
    public void RemoveMoney(int SpentMoney)
    {
        if (Money >= SpentMoney)
        {
            Money -= SpentMoney;
            OnMoneyChanged?.Invoke();
        }
    }

    public void GamePaused(bool isPaused)
    {
        this.isPaused = isPaused;
        OnGamePaused?.Invoke(isPaused);
    }
}

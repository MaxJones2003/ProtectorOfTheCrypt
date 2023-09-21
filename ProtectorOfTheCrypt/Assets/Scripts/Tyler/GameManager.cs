using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<bool> OnGamePaused;
    public static GameManager instance;

    public GameMode GameMode;
    public bool isPaused;
    public int Souls { get; private set; } = 20;
    public int Money { get; private set; } = 250;

    public void Awake()
    {
        instance = this;
        GameMode = gameObject.GetComponent<GameMode>();
    }

    public void RemoveSouls(int LostSouls)
    {
        Souls -= LostSouls;

        if (GameMode is StoryMode)
            GameMode.CheckGameLost();
    }

    // Pass a negative number to give money to the player.
    public bool RemoveMoney(int SpentMoney)
    {
        if (Money >= SpentMoney)
        {
            Money -= SpentMoney;
            return true;
        }
        return false;
    }

    public void GamePaused(bool isPaused)
    {
        this.isPaused = isPaused;
        OnGamePaused?.Invoke(isPaused);
    }
}

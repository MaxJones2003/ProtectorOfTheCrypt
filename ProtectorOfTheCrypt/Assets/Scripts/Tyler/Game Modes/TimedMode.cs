using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class TimedMode : GameMode
{
    public WaveManager waveManager;

    public override bool CheckGameLost()
    {
        return GameManager.instance.Souls == 0;
    }

    public override bool CheckGameWon()
    {
        return WaveManager.CurrentWaveCount + 1 >= waveManager.WavesToSpawn.Count;
    }
}

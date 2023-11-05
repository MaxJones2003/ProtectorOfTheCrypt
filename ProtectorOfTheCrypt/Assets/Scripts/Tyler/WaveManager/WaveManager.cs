using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class WaveManager : MonoBehaviour
{
    [Header("Spawners")]
    public Spawner EnemySpawner;

    public delegate void WaveStarted(Wave wave);
    public static event WaveStarted WaveStartDisplay;

    public delegate void WaveEnded(Wave wave);
    public static event WaveEnded WaveEndDisplay;

    [System.Serializable]
    public class Wave
    {
        public Group EnemyGroup;
        [Range(0f, 20f)]
        public float TimeUntilNextWave;
        public Dialogue Dialogue;
    }
    public List<Wave> WavesToSpawn = new List<Wave>();
    public static Wave CurrentWave;

    [HideInInspector]
    public int CurrentWaveCount = -1;

    public enum SpawnState { SPAWNING, WAITING, FINISHED, HALTED };
    public SpawnState state = SpawnState.WAITING;


    private void Awake()
    {
        EnemySpawner = GetComponent<Spawner>();

        
    }

    public void Start()
    {
        if (GameManager.instance.GameMode is StoryMode)
        {
            StoryMode storyMode = GameManager.instance.GameMode as StoryMode;
            storyMode.waveManager = this;
        }
        else if (GameManager.instance.GameMode is EndlessMode)
        {
            EndlessMode endlessMode = GameManager.instance.GameMode as EndlessMode;
            endlessMode.waveManager = this;
        }
    }


    private void OnEnable()
    {
        EnemySpawner.StoppedSpawningObjects += WaveCompleted;
        Debug.Log(GameManager.instance);
        GameManager.instance.OnGamePaused += PauseSpawning;
    }

    private void OnDisable()
    {
        EnemySpawner.StoppedSpawningObjects -= WaveCompleted;
        GameManager.instance.OnGamePaused -= PauseSpawning;
    }

    public void SpawnFirstWave()
    {
        SpawnWave();
    }

    private void SpawnWave()
    {
        if (CurrentWaveCount + 1 >= WavesToSpawn.Count
            || GameManager.instance.Souls == 0)
        {
            Debug.Log("Done spawning");
            state = SpawnState.FINISHED;
            enabled = false; // turn off the script since its done spawning stuff.
            return;
        }

        CurrentWave = WavesToSpawn[++CurrentWaveCount];

        state = SpawnState.SPAWNING;
        EnemySpawner.SpawnGroup(CurrentWave.EnemyGroup);

        WaveStartDisplay?.Invoke(CurrentWave);
    }

    private void PauseSpawning(bool hasPaused)
    {
        if (hasPaused)
            state = SpawnState.HALTED;
        else
            SpawnWave();
    }

    private void WaveCompleted()
    {
        WaveEndDisplay?.Invoke(CurrentWave);

        if (state == SpawnState.HALTED)
            return;
        else
            state = SpawnState.WAITING;

        Invoke(nameof(SpawnWave), CurrentWave.TimeUntilNextWave);
    }
}

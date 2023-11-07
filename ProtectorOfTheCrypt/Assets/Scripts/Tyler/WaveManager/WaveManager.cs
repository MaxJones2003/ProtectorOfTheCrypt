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

        public Wave(Group enemyGroup, float timeUntilNextWave, Dialogue dialogue)
        {
            EnemyGroup = enemyGroup;
            TimeUntilNextWave = timeUntilNextWave;
            Dialogue = dialogue;
        }
    }
    public List<Wave> WavesToSpawn = new List<Wave>();
    public static Wave CurrentWave;

    [HideInInspector]
    public int CurrentWaveCount = -1;

    public enum SpawnState { SPAWNING, WAITING, FINISHED, HALTED };
    public SpawnState state = SpawnState.WAITING;

    EndlessMode endlessMode;
    StoryMode storyMode;


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
            this.storyMode = storyMode; 

        }
        else if (GameManager.instance.GameMode is EndlessMode)
        {
            EndlessMode endlessMode = GameManager.instance.GameMode as EndlessMode;
            endlessMode.waveManager = this;
            this.endlessMode = endlessMode;
            
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

    public void CreateWave()
    {
        System.Random random = new System.Random();

        WavesToSpawn.Add(new Wave(
            new Group(
                PickRandomEnemyType(random),
                random.Next(1, 11), // needs to be a formulaic value
                random.Next(1, 3)
            ), random.Next(1, 11),
            null));
    }

    public EnemyScriptableObject PickRandomEnemyType(System.Random random)
    {
        int selNum = random.Next(0, 2);
        switch(selNum)
        {
            case 0:
                return endlessMode.basicEnemy;
            case 1:
                return endlessMode.shieldEnemy;
        }
        return null;
    }

    private void SpawnWave()
    {
        if (endlessMode)
            CreateWave();

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

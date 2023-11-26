using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using static WaveManager;

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
    

    public KeyValuePair<float, EnemyScriptableObject>[] probChance = new KeyValuePair<float, EnemyScriptableObject>[2];
    
    private int baseEnemies = 1;
    private int baseTimeBetweenWaves = 1;
    [Header("Endless Mode Enemy Spawn Settings")]

    public float enemySpawnMultiplier;
    public float timeIncreaseMultiplier;

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

            probChance[1] = new KeyValuePair<float, EnemyScriptableObject>(.7f, endlessMode.basicEnemy);
            probChance[0] = new KeyValuePair<float, EnemyScriptableObject>(.3f, endlessMode.shieldEnemy);
        }
    }

    private void OnEnable()
    {
        EnemySpawner.StoppedSpawningObjects += WaveCompleted;
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
        WavesToSpawn.Add(new Wave(
            new Group(
                PickRandomEnemy(),
                baseEnemies + (int)(1f + Mathf.Pow(enemySpawnMultiplier, CurrentWaveCount)),
                UnityEngine.Random.Range(1, 4)
            ), baseTimeBetweenWaves + (int)(1f + Mathf.Pow(timeIncreaseMultiplier, CurrentWaveCount)),
            null));

        WavesToSpawn[WavesToSpawn.Count - 1].TimeUntilNextWave = Mathf.Clamp(WavesToSpawn[WavesToSpawn.Count - 1].TimeUntilNextWave, 0, 6);
        WavesToSpawn[WavesToSpawn.Count - 1].EnemyGroup.NumObjects = Mathf.Clamp(WavesToSpawn[WavesToSpawn.Count - 1].EnemyGroup.NumObjects, 0, 50);
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

    public EnemyScriptableObject PickRandomEnemy()
    {
        float total = 1f;
        float randomPoint = UnityEngine.Random.value * total;
        int index = 0;
        EnemyScriptableObject chosenEnemy;

        for (int i = 0; i < probChance.Length; i++)
        {
            if (randomPoint < probChance[i].Key)
            {
                index = i;
                break;
            }
            else
                randomPoint -= probChance[i].Key;
        }

        chosenEnemy = probChance[index].Value;
        
        return chosenEnemy;
    }
}

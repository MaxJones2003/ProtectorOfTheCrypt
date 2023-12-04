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
    public static int CurrentWaveCount = -1;

    public enum SpawnState { SPAWNING, WAITING, FINISHED, HALTED };
    public SpawnState state = SpawnState.WAITING;

    EndlessMode endlessMode;
    StoryMode storyMode;
    

    public KeyValuePair<float, EnemyScriptableObject>[] probChance = new KeyValuePair<float, EnemyScriptableObject>[4];
    
    private int baseEnemies = 1;
    private int baseTimeBetweenWaves = 1;
    [Header("Endless Mode Enemy Spawn Settings")]

    public float enemySpawnMultiplier;
    public float timeIncreaseMultiplier;

    int enemyCurve;
    public static int goldCurve;

    private void Awake()
    {
        EnemySpawner = GetComponent<Spawner>();
        CurrentWaveCount = -1;
    }

    public void Initialize()
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

            probChance[0] = new KeyValuePair<float, EnemyScriptableObject>(.7f, endlessMode.basicEnemy);
            probChance[1] = new KeyValuePair<float, EnemyScriptableObject>(.2f, endlessMode.shieldEnemy);
            probChance[2] = new KeyValuePair<float, EnemyScriptableObject>(.08f, endlessMode.wizardEnemy);
            probChance[3] = new KeyValuePair<float, EnemyScriptableObject>(.02f, endlessMode.goldEnemy);

            enemyCurve = endlessMode.CurrentSettings.enemyDifficultySettings.enemyCurve;
            WaveManager.goldCurve = endlessMode.CurrentSettings.enemyDifficultySettings.goldCurve;

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
        EnemyScriptableObject randomEnemy = PickRandomEnemy();

        WavesToSpawn.Add(new Wave(
            new Group(
                randomEnemy,
                randomEnemy.spawnsAlone ? 1 : CalculateEnemyCount(),
                UnityEngine.Random.Range(1, 4)
            ), randomEnemy.spawnsAlone ? 5 : CalculateTimeBetweenWaves(),
            null));
    }

    private int CalculateEnemyCount()
    {
        int x = CurrentWaveCount;
        //Debug.Log((int)(enemyCurve / (1f + Mathf.Exp((-.17f * x) + 2))));
        return (int)(enemyCurve / (1f + Mathf.Exp((-.17f * x) + 2)));
    }
    private int CalculateTimeBetweenWaves()
    {
        int x = CurrentWaveCount;
        return (int)(enemyCurve*2 / (1f + Mathf.Exp((-.15f * x) + 2)));
    }

    public static int CalculateMoneyToDrop()
    {
        int x = CurrentWaveCount;
        return (int)((WaveManager.goldCurve / (1f + Mathf.Exp((-.2f * x) + 2.5f))) + 22);
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

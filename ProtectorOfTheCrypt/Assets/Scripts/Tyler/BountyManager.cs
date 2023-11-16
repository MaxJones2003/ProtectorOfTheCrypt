//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Linq;

//public class BountyManager : MonoBehaviour, ISaveable
//{
//    public ScenesData sceneDB;

//    public static BountyManager instance;

//    [Header("Debug")]
//    public bool OnlySpawnSpecialEnemies;
//    public bool CanSpawnFlagship;

//    [Header("ObjectsToSpawn")]
//    public GameObject warpGatePrefab;
//    public GameObject flagshipPrefab;
//    public GameObject ancientTotemPrefab;

//    [Header("Spawned Objects")]
//    public List<GameObject> spawnedEnemies = new List<GameObject>();

//    [Header("Spawners")]
//    public ObjectSpawner[] shipSpawners;
//    public ObjectSpawner[] warpGateSpawners;

//    public delegate void WaveStarted();
//    public static event WaveStarted WaveStartDisplay;

//    public delegate void WaveEnded();
//    public static event WaveStarted WaveEndDisplay;

    

//    [System.Serializable]
//    public class Wave
//    {
//        public int count;
//        public int waveNumber;
//        public List<ObjectSpawner> activeSpawnBoxes;
//        public float startTime;
//        public int accumulatedCost;
//    }
//    public Wave currentWave;

//    public enum BountyState { Smiley, Mid, Frown, AbsolutelyDiabolical };
//    public BountyState bState = BountyState.Smiley;
//    public enum SpawnState { SPAWNING, WAITING, COUNTING };
//    public SpawnState state = SpawnState.COUNTING;
    
//    [Header("Enemy Spawn Probabilities")]
//    public float baseSpawnRate = 0.005f;
//    public KeyValuePair<float, GameObject>[] probChance;
//    public int spawnRateIncThreshold = 72000;
    
//    [Header("Bounty Stats")]
//    public int bounty;
//    public int bountyIncrement;
//    public int enemiesRequiredForNextRound = 2;
//    public int waveCount = 0;

//    [SerializeField]
//    private int baseEnemies;

//    [Range(0f, 20f)]
//    public float timeBetweenWaves;
//    [Range(1f, 1.20f)]
//    public float enemySpawnMultiplier;
//    public float maxWaveLength;

//    private float searchCountdown = 1f;

//    public void Initialize()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void OnApplicationQuit()
//    {
//        sceneDB.currentLevel = sceneDB.levels.Find(x => x.sceneName == "StarSystem");
//    }
//    public void Awake()
//    {
//        Initialize();
//        DetermineBountyState();
//    }
//    public void Start()
//    {
//        SpawnWarpGate();
//        if(CanSpawnFlagship)
//        {
//            SpawnFlagship();
//        }
//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }

//    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        StopAsteroids();

//        if (scene.name.Equals("RestStation") || scene.name.Equals("MapScene") ||
//            scene.name.Equals("MenuScene") || scene.name.Equals("IntroScene") ||
//            scene.name.Equals("CreditsScene"))
//        {
            
//            gameObject.SetActive(false);
//            return;
//        }

//        gameObject.SetActive(true);
//        SpawnWarpGate();

//        if(scene.name.Equals("DeadSystem"))
//        {
//            SpawnAncientTotem();
//        }

//        if (scene.name.Equals("AsteroidBelt"))
//        {
//            SpawnAsteroids();
//        }

//        if (MapManager.instance.map.currentNode == MapManager.instance.map.flagshipNode
//           || CanSpawnFlagship)
//        {
//            SpawnFlagship();
//        }

//        spawnedEnemies.Clear();
//    }
//    private void Update()
//    {
//        UpdateBounty();
//        HandleEnemyWaves();
//        SpawnCivilians();
//    }
    
//    private float waveCountdown; // Used for keeping track of wave timings.
//    private void HandleEnemyWaves()
//    {
//        if (state == SpawnState.WAITING)
//        {
//            if (WaveOver())
//            {
//                WaveCompleted();
//                return;
//            }
                
//            else
//                return;
//        }
//        if (waveCountdown <= 0)
//        {
//            if (state != SpawnState.SPAWNING)
//            {
//                SpawnWave(CreateWave());
//            }
//        }
//        else
//        {
//            waveCountdown -= Time.deltaTime;
//        }
//    }

//    private Wave CreateWave()
//    {
//        Wave wave = new Wave();
//        wave.waveNumber = waveCount;
//        wave.count = baseEnemies + (int)(1f + Mathf.Pow(enemySpawnMultiplier, wave.waveNumber));
//        wave.activeSpawnBoxes = GetRandomSpawnBoxes();
//        wave.startTime = Time.time;
//        waveCount++;
//        return wave;
//    }

//    private List<ObjectSpawner> GetRandomSpawnBoxes()
//    {
//        if (bState.Equals(BountyState.AbsolutelyDiabolical))
//            return shipSpawners.ToList<ObjectSpawner>();

//        List<ObjectSpawner> spawners = new List<ObjectSpawner>();
//        System.Random a = new System.Random();

//        for (int i = 0; i < ((int)bState) + 1; i++)
//        {
//            ObjectSpawner spawner = shipSpawners[a.Next(0, 4)];
//            while (spawners.Contains(spawner))
//            {
//                spawner = shipSpawners[a.Next(0, 4)];
//            }

//            spawners.Add(spawner);
//        }

//        return spawners;
//    }
//    private void SpawnWave(Wave _wave)
//    {
//        currentWave = _wave;
//        state = SpawnState.SPAWNING;

//        for (int i = 0; i < _wave.count; i++)
//        {
//            if (!CanAddCost(0))
//                break;
//            SpawnEnemy(_wave);
//        }
//        WaveStartDisplay();
        
//        state = SpawnState.WAITING;
//    }

//    private void WaveCompleted()
//    {
//        state = SpawnState.COUNTING;
//        if(WaveEndDisplay != null)
//            WaveEndDisplay();
//        waveCountdown = timeBetweenWaves;
//    }
//    public bool WaveOver()
//    {
//        searchCountdown -= Time.deltaTime;
//        if(searchCountdown <= 0f)
//        {
//            searchCountdown = 1f;
//            if ((spawnedEnemies.Count() <= enemiesRequiredForNextRound) ||
//                (Time.time - currentWave.startTime) >= maxWaveLength)
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    void SpawnEnemy(Wave wave)
//    {
//        ObjectSpawner spawner = wave.activeSpawnBoxes[UnityEngine.Random.Range(0, wave.activeSpawnBoxes.Count)];
//        CalculateSpawnChances();
//        GameObject spawnedEnemy = spawner.SpawnEnemy(sceneDB.currentLevel.PickRandomEnemy());
//        spawnedEnemies.Add(spawnedEnemy);
//    }

//    public void AddSpawnCost(int cost)
//    {
//        currentWave.accumulatedCost += cost;

//        if (currentWave.accumulatedCost > currentWave.count)
//            Debug.LogError("Cost overflow!");
//    }

//    public bool CanAddCost(int cost)
//    {
//        int newCost = currentWave.accumulatedCost + cost;
//        if (newCost < currentWave.count)
//            return true;
//        else return false;
//    }

//    float civSpawnTimer = 0;
//    float timeNeededToSpawnCiv = 0;
//    private void SpawnCivilians()
//    {
//        civSpawnTimer += Time.deltaTime;
//        if (civSpawnTimer >= timeNeededToSpawnCiv)
//        {
//            civSpawnTimer = 0;
//            ObjectSpawner spawnEdge = shipSpawners[UnityEngine.Random.Range(0, shipSpawners.Length)];
//            spawnEdge.SpawnCivilian(sceneDB.currentLevel.PickRandomCivilian());
//            timeNeededToSpawnCiv = sceneDB.currentLevel.CalculateCivilianSpawnTime();
//        }
//    }

//    public void SpawnWarpGate()
//    {
//        ObjectSpawner spawnEdge = warpGateSpawners[UnityEngine.Random.Range(0, warpGateSpawners.Length)];
//        spawnEdge.SpawnLevelObject(warpGatePrefab);
//    }
//    public void SpawnAncientTotem()
//    {
//        ObjectSpawner spawnEdge = warpGateSpawners[UnityEngine.Random.Range(0, warpGateSpawners.Length)];
//        spawnEdge.SpawnLevelObject(ancientTotemPrefab);
//    }
//    public void SpawnFlagship()
//    {
//        ObjectSpawner spawnEdge = shipSpawners[UnityEngine.Random.Range(0, shipSpawners.Length)];
//        spawnEdge.SpawnLevelObject(flagshipPrefab);
//        AudioManager.Instance.BossMusic();
//    }    

//    public void SpawnAsteroids()
//    {
//        foreach (ObjectSpawner spawner in shipSpawners)
//        {
//            spawner.StartSpawningAsteroids();
//        }
//    }

//    public void StopAsteroids()
//    {
//        foreach (ObjectSpawner spawner in shipSpawners)
//        {
//            spawner.StopSpawningAsteroids();
//        }
//    }

//    float gameTime;
//    private void UpdateBounty()
//    {
//        gameTime += Time.deltaTime;
//        if (gameTime >= 1)
//        {
//            AddBounty((int) (bountyIncrement * PlayerStats.instance._bountyMultiplier.Value));
//            gameTime = 0;
//        }
//    }

//    public void AddBounty(int bountyPoints)
//    {
//        bounty += bountyPoints;
//        DetermineBountyState();
//    }

//    public void DetermineBountyState()
//    {
//        switch (bounty)
//        {
//            case var expression when bounty >= 150000:
//                bState = BountyState.AbsolutelyDiabolical;
//                break;
//            case var expression when bounty >= 100000:
//                bState = BountyState.Frown;
//                break;

//            case var expression when bounty >= 50000:
//                bState = BountyState.Mid;
//                break;
//            default:
//                bState = BountyState.Smiley;
//                break;
//        }
//    }

//    private float spawnChance = 0;
//    private void CalculateSpawnChances()
//    {
//        Level level = sceneDB.currentLevel;
//        UpdateSpawnChances();
//        probChance = new KeyValuePair<float, GameObject>[level.specialEnemyTypes.Length + level.enemyTypes.Length];

//        for (int i = 0; i < level.specialEnemyTypes.Length; i++)
//        {
//            probChance[i] = new KeyValuePair<float, GameObject>(spawnChance, level.specialEnemyTypes[i]);
//        }
//        for (int i = 0; i < level.enemyTypes.Length; i++)
//        {
//            probChance[i + level.specialEnemyTypes.Length] = new KeyValuePair<float, GameObject>((1 - (spawnChance * level.specialEnemyTypes.Length)), level.enemyTypes[i]);
//        }
//    }

//    int lastBounty = 0;
//    private void UpdateSpawnChances()
//    {
//        // Spawn Chance has reached max, dont update.
//        if (spawnChance >= .275f)
//            return;
//        if(bounty >= lastBounty + spawnRateIncThreshold)
//        {
//            lastBounty = bounty;
//            spawnChance += baseSpawnRate;
//        }
//    }

//    public void RemoveEnemy(GameObject enemy)
//    {
//        spawnedEnemies.Remove(enemy);
//    }

//    public void AddEnemy(GameObject enemy)
//    {
//        spawnedEnemies.Add(enemy);
//    }

//    public void RestoreBounty()
//    {
//        bounty = 0;
//        waveCount = 0;
//        lastBounty = 0;
//        spawnChance = 0;
//        currentWave = null;
//        CanSpawnFlagship = false;
//        OnlySpawnSpecialEnemies = false;
//        DetermineBountyState();
//    }
//    public object SaveState()
//    {
//        return new SaveData
//        {
//            bounty = this.bounty,
//            waveCount = this.waveCount,
//        };
//    }

//    public void LoadState(object state)
//    {
//        var saveData = (SaveData)state;

//        this.bounty = saveData.bounty;
//        this.waveCount = saveData.waveCount;
//    }

//    [System.Serializable]
//    private struct SaveData
//    {
//        public int bounty;
//        public int waveCount;
//    }
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "LevelObject", menuName = "Scene Data/Level")]
//public class Level : GameScene
//{
//    public enum LevelType { RestStation, Obstacle, Normal, Dead, Hive, Defense };
//    public LevelType levelType;
    
//    [Header("Enemies")]
//    public GameObject[] enemyTypes;

//    [Header("Special Enemies")]
//    public GameObject[] specialEnemyTypes;

//    [Header("Civilians")]
//    public GameObject[] civilianTypes;
//    public float[] spawnChances;
//    [SerializeField] private float minSpawnTime;
//    [SerializeField] private float maxSpawnTime;

//    [Header("Required Scrap")]
//    public List<Scrap> requiredScrap;

//    [System.Serializable]
//    public struct Scrap
//    {
//        public Droppable droppable;
//        public int minCount;
//        public int maxCount;
//        public int[] minCountInf;
//        public int[] maxCountInf;
//    }

//    public GameObject PickRandomEnemy()
//    {
//        float total = 1f;
//        float randomPoint = UnityEngine.Random.value * total;
//        int index = 0;
//        GameObject chosenEnemy;
//        if (BountyManager.instance.OnlySpawnSpecialEnemies)
//            return specialEnemyTypes[Random.Range(0, specialEnemyTypes.Length)];

//        for (int i = 0; i < BountyManager.instance.probChance.Length; i++)
//        {
//            if (randomPoint < BountyManager.instance.probChance[i].Key)
//            {
//                index = i;
//                break;
//            }
//            else
//                randomPoint -= BountyManager.instance.probChance[i].Key;
//        }

//        if (index < specialEnemyTypes.Length)
//        {
//            if(BountyManager.instance.CanAddCost(2) &&
//               (MapManager.instance.map.currentNode != MapManager.instance.map.flagshipNode))
//            {
//                BountyManager.instance.AddSpawnCost(2);
//                chosenEnemy = BountyManager.instance.probChance[Random.Range(0, specialEnemyTypes.Length)].Value;
//            }
//            else
//            {
//                BountyManager.instance.AddSpawnCost(1);
//                chosenEnemy = BountyManager.instance.probChance[BountyManager.instance.probChance.Length - 1].Value;
//            }
//        }
//        else
//        {
//            BountyManager.instance.AddSpawnCost(1);
//            chosenEnemy = BountyManager.instance.probChance[BountyManager.instance.probChance.Length - 1].Value; // <= no enemies spawning for some reason
//        }
//        return chosenEnemy;
//    }

//    public GameObject PickRandomCivilian()
//    {
//        float total = 1f;
//        float randomPoint = Random.value * total;
//        float currentChance = 0f;
//        for (int i = 0; i < civilianTypes.Length; i++)
//        {
//            currentChance += spawnChances[i];
//            if (randomPoint <= currentChance)
//            {
//                if (!civilianTypes[i])
//                    Debug.Log("Invalid civ at index: " + i);
//                return civilianTypes[i];
//            }
//        }

//        return civilianTypes[0];
//    }

//    public float CalculateCivilianSpawnTime()
//    {
//        return UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
//    }

//    public double GenerateRandomWeight()
//    {
//        return UnityEngine.Random.Range(0, 100);
//    }
//}

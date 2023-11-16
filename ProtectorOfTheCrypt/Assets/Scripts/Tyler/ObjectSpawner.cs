//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class ObjectSpawner : MonoBehaviour
//{
//    public enum CameraSide { Left, Right, Up, Down};
//    public CameraSide side;
//    public BoxCollider2D spawnBox;
//    public GameObject AsteroidPrefab;
    
//    public void Start()
//    {
//        //InvokeRepeating("SpawnCivilians", 0f, civilianSpawnTime);
//    }
//    public void StartSpawningAsteroids()
//    {
//        InvokeRepeating(nameof(SpawnAsteroid), 0, Random.Range(3, 6));
//    }

//    public void StopSpawningAsteroids()
//    {
//        CancelInvoke(nameof(SpawnAsteroid));
//    }

//    public GameObject SpawnCivilian(GameObject civ)
//    {
//        Bounds colliderBounds = spawnBox.bounds;
//        Vector3 colliderCenter = colliderBounds.center;

//        GameObject spawnedCivilian = null;
//        if (side.Equals(CameraSide.Up) || side.Equals(CameraSide.Down))
//        {
//            spawnedCivilian = Instantiate(civ, new Vector2(DetermineSpawnPointX(colliderBounds, colliderCenter), colliderCenter.y), Quaternion.identity);
//            spawnedCivilian.GetComponent<CivilianShipController>().MoveToPosition(new Vector2(DetermineSpawnPointX(colliderBounds, colliderCenter), -colliderCenter.y));
//        }
//        else if (side.Equals(CameraSide.Right) || side.Equals(CameraSide.Left))
//        {
//            spawnedCivilian = Instantiate(civ, new Vector2(colliderCenter.x, DetermineSpawnPointY(colliderBounds, colliderCenter)), Quaternion.identity);
//            spawnedCivilian.GetComponent<CivilianShipController>().MoveToPosition(new Vector2(-colliderCenter.x, DetermineSpawnPointY(colliderBounds, colliderCenter)));
//        }

//        return spawnedCivilian;
//    }

//    public GameObject SpawnEnemy(GameObject enemyPrefab)
//    {
//        Bounds colliderBounds = spawnBox.bounds;
//        Vector3 colliderCenter = colliderBounds.center;

//        if (side.Equals(CameraSide.Up) || side.Equals(CameraSide.Down))
//        {
//            enemyPrefab = Instantiate(enemyPrefab, new Vector2(DetermineSpawnPointX(colliderBounds, colliderCenter), colliderCenter.y), Quaternion.identity);
//        }
//        else if (side.Equals(CameraSide.Right) || side.Equals(CameraSide.Left))
//        {
//            enemyPrefab = Instantiate(enemyPrefab, new Vector2(colliderCenter.x, DetermineSpawnPointY(colliderBounds, colliderCenter)), Quaternion.identity);
//        }
//        return enemyPrefab;
//    }

//    public void SpawnAsteroid()
//    {
//        Bounds colliderBounds = spawnBox.bounds;
//        Vector3 colliderCenter = colliderBounds.center;

//        GameObject spawnedAsteroid = null;
//        if (side.Equals(CameraSide.Up) || side.Equals(CameraSide.Down))
//        {
//            spawnedAsteroid = Instantiate(AsteroidPrefab, new Vector2(DetermineSpawnPointX(colliderBounds, colliderCenter), colliderCenter.y), Quaternion.identity);
//            spawnedAsteroid.GetComponent<Asteroid>().destination = new Vector2(DetermineSpawnPointX(colliderBounds, colliderCenter), -colliderCenter.y);
//            spawnedAsteroid.GetComponent<Asteroid>().SendFlying();
//        }
//        else if (side.Equals(CameraSide.Right) || side.Equals(CameraSide.Left))
//        {
//            spawnedAsteroid = Instantiate(AsteroidPrefab, new Vector2(colliderCenter.x, DetermineSpawnPointY(colliderBounds, colliderCenter)), Quaternion.identity);
//            spawnedAsteroid.GetComponent<Asteroid>().destination = new Vector2(-colliderCenter.x, DetermineSpawnPointY(colliderBounds, colliderCenter));
//            spawnedAsteroid.GetComponent<Asteroid>().SendFlying();
//        }
//    }

//    public void SpawnLevelObject(GameObject prefab)
//    {
//        Bounds colliderBounds = spawnBox.bounds;
//        Vector3 colliderCenter = colliderBounds.center;

//        Instantiate(prefab, new Vector2(DetermineSpawnPointX(colliderBounds, colliderCenter),
//                    DetermineSpawnPointY(colliderBounds, colliderCenter)), Quaternion.identity);
//    }

//    public float DetermineSpawnPointX(Bounds colliderBounds, Vector3 colliderCenter)
//    {
//        float randomX = Random.Range(colliderCenter.x - colliderBounds.extents.x, colliderCenter.x + colliderBounds.extents.x);

//        return randomX;
//    }

//    public float DetermineSpawnPointY(Bounds colliderBounds, Vector3 colliderCenter)
//    {
//        float randomY = Random.Range(colliderCenter.y - colliderBounds.extents.y, colliderCenter.y + colliderBounds.extents.y);

//        return randomY;
//    }
//}

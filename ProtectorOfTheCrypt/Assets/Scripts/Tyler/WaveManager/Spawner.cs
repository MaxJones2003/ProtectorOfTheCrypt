using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<Vector3> Path = new List<Vector3>();

    public delegate void StoppedSpawning();
    public event StoppedSpawning StoppedSpawningObjects;

    public List<GameObject> SpawnedObjects = new List<GameObject>();
    public void SpawnGroup(Group group)
    {
        StartCoroutine(SpawnObject(group.Object, group.NumObjects, group.TimeBetweenSpawning));
    }

    int numSpawned = 0; // A Local Variable to keep track of our current spawn count.
    public IEnumerator SpawnObject(EnemyScriptableObject EnemyToSpawn, int numToSpawn = 1, float TimeBetweenSpawning = 1f)
    {
        while(numSpawned < numToSpawn)
        {
            yield return new WaitForSeconds(TimeBetweenSpawning);
            numSpawned++;
            SpawnedObjects.Add(EnemyToSpawn.Spawn(transform, this, Path, this));
        }

        numSpawned = 0; // Reset.
        StoppedSpawningObjects?.Invoke();
    }
}

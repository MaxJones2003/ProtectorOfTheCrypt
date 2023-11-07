using System;
using UnityEngine;
[System.Serializable]
public class Group
{
    public EnemyScriptableObject Object;
    public int NumObjects;
    public float TimeBetweenSpawning;

    public Group(EnemyScriptableObject obj, int NumObjects, float TimeBetweenSpawning)
    {
        Object = obj;
        this.NumObjects = NumObjects;
        this.TimeBetweenSpawning = TimeBetweenSpawning;
    }
}

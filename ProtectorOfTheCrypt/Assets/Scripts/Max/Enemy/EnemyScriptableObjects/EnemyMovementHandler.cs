using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementHandler : MonoBehaviour
{
    private EnemyScriptableObject enemy = null;

    private bool paused;
    private List<Vector3> path = new List<Vector3>();
    private Vector3 target;
    private int waypointIndex = 0;
    public float baseSpeed;
    [HideInInspector]
    public Spawner spawner;

    private Transform model;

    public void Awake()
    {
        model = transform.GetChild(0);
        GameManager.instance.OnGamePaused += UpdateGamePaused;
    }
    public void Initialize(EnemyScriptableObject EnemyToSet, List<Vector3> Path, float BaseSpeed, Spawner _spawner)
    {
        enemy = EnemyToSet;
        path = Path;
        target = path[1];
        baseSpeed = BaseSpeed;
        spawner = _spawner;
    }

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        if(paused)
            return;
        target.y = transform.position.y;
        Vector3 dir = target - transform.position;
        dir.Normalize();
        transform.Translate(dir * baseSpeed * Time.deltaTime);
        model.LookAt(target);

        if (Vector3.Distance(transform.position, target) <= 0.1f)
        {
            waypointIndex++;
            if (waypointIndex >= path.Count)
            {
                // Reached the last waypoint, stop moving
                GameManager.instance.RemoveSouls(5);
                spawner.SpawnedObjects.Remove(gameObject);
                if (GameManager.instance.GameMode is StoryMode)
                {
                    if (GameManager.instance.GameMode.CheckGameWon())
                        GameManager.instance.GameMode.OnGameWon();
                }
                Destroy(gameObject);
                return;
            }
            target = path[waypointIndex];
        }
    }

    private void UpdateGamePaused(bool isPaused)
    {
        paused = isPaused;
    }
    private void OnDestroy()
    {
        GameManager.instance.OnGamePaused -= UpdateGamePaused;
    }
}

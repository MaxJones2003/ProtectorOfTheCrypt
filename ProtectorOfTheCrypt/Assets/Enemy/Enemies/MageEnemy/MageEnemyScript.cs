using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyScript : MonoBehaviour
{
    public LayerMask enemyLayerMask;
    [SerializeField] private GameObject rangeIndicator;
    public float boostRadius = 3f;
    public float boostMultiplier = 1.75f;

    public List<GameObject> enemiesInRange;
    public List<GameObject> foundEnemiesThisFrame;

    public List<GameObject> boostedEnemies;

    private void Awake()
    {
        enemiesInRange = new List<GameObject>();
        foundEnemiesThisFrame = new List<GameObject>();
    }

    private void Update()
    {
        CheckArea();
    }

    private void CheckArea()
    {
        RaycastHit[] hits = Physics.SphereCastAll(new Ray(rangeIndicator.transform.position, Vector3.up), boostRadius, 0, enemyLayerMask, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if(hit.collider == null || hit.collider.gameObject == this || enemiesInRange.Contains(hit.collider.gameObject)) continue;
            Debug.Log("Found enemy");
            GameObject enemyGO = hit.collider.gameObject;
            enemiesInRange.Add(enemyGO);
            foundEnemiesThisFrame.Add(enemyGO);
            TryBoost(enemyGO);
            AddEnemySubscription(enemyGO);
        }
        foreach(GameObject enemy in enemiesInRange)
        {
            if(foundEnemiesThisFrame.Contains(enemy)) continue;
            // remove boost

            // remove from list
            enemiesInRange.Remove(enemy);
        }

        
    }
    private void AddEnemySubscription(GameObject enemy)
    {
        EnemyMovementHandler moveScript = enemy.GetComponent<EnemyMovementHandler>();
        if (moveScript != null)
        {
            Debug.LogError("Somehow I found an enemy but not its movement script.");
            return;
        }
        moveScript.Slowed += EnemySlowedListiner;
    }
    private void EnemySlowedListiner((bool isSlowed, GameObject go) enemy)
    {
        if(enemy.isSlowed)
        {
            // This enemy got slowed, remove it from the boostedEnemies
            boostedEnemies.Remove(enemy.go);
        }
        else
        {
            // this enemy is in range and is no longer slowed, add it to boosted enemies and give it a boost
            TryBoost(enemy.go);
        }
    }
    private void TryBoost(GameObject enemy)
    {
        Debug.Log(enemy.name);
        if (enemy.TryGetComponent<EnemyMovementHandler>(out var moveScript))
        {
            Debug.LogError("Somehow I found an enemy but not its movement script.");
            return;
        }
        moveScript.BoostSpeed(boostMultiplier);
        boostedEnemies.Add(enemy);
    }
    private void RemoveBoost(GameObject enemy)
    {
        if (enemy.TryGetComponent<EnemyMovementHandler>(out var moveScript))
        {
            Debug.LogError("Somehow I found an enemy but not its movement script.");
            return;
        }
        moveScript.UnBoostSpeed();
        boostedEnemies.Remove(enemy);
    }










    private void SetIndicatorRadius()
    {
        rangeIndicator.transform.localScale = new Vector3(boostRadius, 1, boostRadius);
    }

    private void OnValidate()
    {
        SetIndicatorRadius();
    }
}

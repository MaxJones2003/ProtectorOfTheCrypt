using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MageEnemyScript : MonoBehaviour
{
    public LayerMask enemyLayerMask;
    [SerializeField] private GameObject rangeIndicator;
    public float boostRadius = 3f;
    public float boostMultiplier = 1.75f;


    public List<GameObject> boostedEnemies;    

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
    public void TryBoost(GameObject enemy)
    {
        if (!enemy.TryGetComponent<EnemyMovementHandler>(out var moveScript))
        {
            Debug.LogError("Somehow I found an enemy but not its movement script.");
            return;
        }
        Debug.Log("Boosting " + enemy.name);
        moveScript.BoostSpeed(boostMultiplier);
        boostedEnemies.Add(enemy);
        AddEnemySubscription(enemy);
    }
    private void AddEnemySubscription(GameObject enemy)
    {
        EnemyMovementHandler moveScript = enemy.GetComponent<EnemyMovementHandler>();
        if (moveScript == null)
        {
            Debug.LogError("Somehow I found an enemy but not its movement script.");
            return;
        }
        moveScript.Slowed += EnemySlowedListiner;
        moveScript.Died += RemoveEnemySubscriptions;
    }
    public void RemoveBoost(GameObject enemy)
    {
        if (enemy.TryGetComponent<EnemyMovementHandler>(out var moveScript))
        {
            Debug.Log("Unboosting " + enemy.name);
            moveScript.UnBoostSpeed();
        }
        Debug.Log("Removing " + enemy.name + " from boosted enemies");
        boostedEnemies.Remove(enemy);
        RemoveEnemySubscriptions(enemy);
    }

    private void RemoveEnemySubscriptions(GameObject enemy)
    {
        EnemyMovementHandler moveScript = enemy.GetComponent<EnemyMovementHandler>();
        if (moveScript == null)
        {
            Debug.LogError("Somehow I found an enemy but not its movement script.");
            return;
        }
        moveScript.Slowed -= EnemySlowedListiner;
        moveScript.Died -= RemoveEnemySubscriptions;
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

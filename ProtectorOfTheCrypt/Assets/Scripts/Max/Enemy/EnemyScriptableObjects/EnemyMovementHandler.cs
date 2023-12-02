using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovementHandler : MonoBehaviour, ISlowable
{
    private EnemyScriptableObject enemy = null;

    private bool paused;
    private List<Vector3> path = new List<Vector3>();
    private Vector3 target;
    private int waypointIndex = 0;
    public float BaseSpeed;
    public float speed;
    [HideInInspector]
    public Spawner spawner;

    private Transform model;

    private CharacterController characterController; // Add Character Controller component

    [Header("Animation")]
    public float hopAngle = 15.0f;
    public float hopSpeed = 2.0f;
    public float wobbleAmount = 12f;
    private Quaternion originalRotation;
    private float timeElapsed = 0.0f;

    private Coroutine SlowCoroutine;

    public void Awake()
    {
        model = transform.GetChild(0);
        characterController = GetComponent<CharacterController>(); // Initialize the Character Controller
        characterController.enabled = false;
    }

    public void OnEnable()
    {
        GameManager.instance.OnGamePaused += UpdateGamePaused;
    }

    private void OnDisable()
    {
        GameManager.instance.OnGamePaused -= UpdateGamePaused;
    }
    int hungerModifier = 1;
    public void Initialize(EnemyScriptableObject EnemyToSet, List<Vector3> Path, float BaseSpeed, Spawner _spawner)
    {
        float speedModifier = 1;
        if(GameManager.instance.GameMode is EndlessMode)
        {
            EndlessMode endlessMode = (EndlessMode)GameManager.instance.GameMode;
            speedModifier = endlessMode.CurrentSettings.enemyDifficultySettings.speedMultiplier;
            hungerModifier = endlessMode.CurrentSettings.enemyDifficultySettings.hungerMultiplier;
        }
        enemy = EnemyToSet;
        path = Path;
        target = path[1];
        this.BaseSpeed = BaseSpeed * speedModifier;
        speed = this.BaseSpeed;
        spawner = _spawner;

        originalRotation = model.transform.rotation;
        characterController.enabled = true;
    }

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        if (paused)
            return;
        target.y = transform.position.y;
        Vector3 dir = target - transform.position;
        dir.Normalize();
        
        // Smoothly rotate towards the next waypoint
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime);

        // Use Character Controller for movement
        characterController.Move(dir * speed * Time.deltaTime);
        ModelAnimation();
        

        if (Vector3.Distance(transform.position, target) <= 0.1f)
        {
            waypointIndex++;
            if (waypointIndex >= path.Count)
            {
                // Reached the last waypoint, stop moving
                GameManager.instance.RemoveSouls(enemy.Hunger * hungerModifier);
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
            if(waypointIndex - 1 >= 0)
                RotateGameObject(target, path[waypointIndex-1]);
        }
    }

    private void RotateGameObject(Vector3 target, Vector3 lastTarget)
    {
        if(target.x > lastTarget.x)
        {
            // Rotate to look right (rotate y value to 0)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            
        }
        else if(target.x < lastTarget.x)
        {
            // Rotate to look left (rotate y value to 180)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if(target.z > lastTarget.z)
        {
            // Rotate to look up (rotate y value to -90)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if(target.z < lastTarget.z)
        {
            // Rotate to look down (rotate y value to 90)
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
    }

    private void ModelAnimation()
    {
        float angle = Mathf.Sin(timeElapsed * hopSpeed) * hopAngle;
        float wobble = Mathf.Cos(timeElapsed * hopSpeed * 2) * wobbleAmount;
        Quaternion hopRotation = Quaternion.Euler(Vector3.up * angle) * Quaternion.Euler(Vector3.forward * wobble);
        model.transform.rotation = hopRotation * originalRotation * transform.rotation;

        timeElapsed += Time.deltaTime;
    }

    private void UpdateGamePaused(bool isPaused)
    {
        paused = isPaused;
    }
    [HideInInspector] public bool isSpedUp, isSlowedDown;
    public void Slow(AnimationCurve SlowCurve)
    {
        if (SlowCoroutine != null)
        {
            StopCoroutine(SlowCoroutine);
            isSlowedDown = false;
            Slowed.Invoke((false, gameObject));
        }
        SlowCoroutine = StartCoroutine(SlowDown(SlowCurve));
    }

    private IEnumerator SlowDown(AnimationCurve SlowCurve)
    {
        Slowed.Invoke((true, gameObject));
        isSpedUp = false;
        isSlowedDown = true;
        float time = 0;
        Debug.Log("Slowing " + speed);
        while (time < SlowCurve.keys[^1].time)
        {
            speed = BaseSpeed * SlowCurve.Evaluate(time);
            time += Time.deltaTime;
            yield return null;
        }
        Debug.Log("UnSlowing " + speed);
        speed = BaseSpeed;
        Slowed.Invoke((false, gameObject));
    }
    public event Action<(bool,GameObject)> Slowed;
    public void BoostSpeed(float increaseBy)
    {
        isSpedUp = true;
        speed = BaseSpeed * increaseBy;
    }
    public void UnBoostSpeed()
    {
        isSpedUp = false;
        speed = BaseSpeed;
    }

}

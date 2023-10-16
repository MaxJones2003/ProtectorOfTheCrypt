using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for moving the projectile and handeling collision. This script is set up to work with the ObjectPooling script.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody Rigidbody;
    public TowerScriptableObject tower;
    private bool isTracking = false;
    private Transform target;
    private float speed;
    private Vector3 direction;

    [field: SerializeField]
    public Vector3 SpawnLocation
    {
        get;
        private set;
    }
    [SerializeField]
    private float DelayedDisableTime = 10f;
    private float elapsedTime = 0f; // If the game gets paused, this freezes the bullets and the timer
    public delegate void CollisionEvent(Bullet Bullet, Collision Collision);
    public event CollisionEvent OnCollision;
    private Quaternion originalRotation;
    private Transform model;

    private bool paused;

    private void Awake() 
    {
        Rigidbody = GetComponent<Rigidbody>();
        gameObject.layer = LayerMask.NameToLayer("Projectile");
        GameManager.instance.OnGamePaused += UpdateGamePaused;
        model = transform.GetChild(0);
        originalRotation = model.rotation;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        GameManager.instance.OnGamePaused -= UpdateGamePaused;
        //OnCollision -= tower.HandleBulletCollision;
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        isTracking = false;
        target = null;
        OnCollision = null;
        Destroy(gameObject);
    }

    private void UpdateGamePaused(bool isPaused)
    {
        paused = isPaused;
    }

    private void Update()
    {
        if(paused) return;

        Move();
        Rotate();
    }
    private void Move()
    {
        if(isTracking)
        {
            //projectile is active: move towards the target
            if(target != null)
            {
                direction = target.position - transform.position;
                transform.position += direction.normalized * speed * Time.deltaTime;
            }
            else
            {
                transform.position += direction.normalized * speed * Time.deltaTime;
            }
                    
        } 
    }
    private void Rotate()
    {
        Vector3 lookDirection = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection) * originalRotation;
        model.rotation = Quaternion.Slerp(model.rotation, targetRotation, Time.deltaTime);
    }

    /// <summary>
    /// Sets the projectile to the tower projectile spawn point and sets the target.
    /// </summary>
    /// <param name="Speed"></param>
    /// <param name="Target"></param>
    public void Spawn(float Speed, Transform Target)
    {
        SpawnLocation = transform.position;
        speed = Speed;
        target = Target;
        isTracking = true;
        Vector3 lookDirection = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection) * originalRotation;
        model.rotation = targetRotation;
        StartCoroutine(DelayedDisable(DelayedDisableTime));
    }

    /// <summary>
    /// If the projectile is active for "Time" without hitting an object: Disable it.
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    private IEnumerator DelayedDisable(float time)
    {
        float startTime = Time.time;

        while (elapsedTime < time)
        {
            if (!paused)
            {
                // Perform some actions here.
                elapsedTime = Time.time - startTime;
            }

            yield return null; // Yielding null means the coroutine will run in the same frame.
        }
        Debug.Log("I didn't hit anything :(");
        OnCollisionEnter(null);
    }

    private void OnCollisionEnter(Collision Collision) 
    {
        OnCollision?.Invoke(this, Collision);
    }

    
    private void OnDestroy()
    {
        
    }
}

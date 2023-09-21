using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ShootMonoBehaviour : MonoBehaviour
{
    public ImpactType ImpactType;
    public DamageConfigScriptableObject DamageConfig;
    public ProjectileConfigurationScriptableObject ProjectileConfig;
    public TrailConfigurationScriptableObject TrailConfig;
    public AuidoConfigScriptableObject AudioConfig;

    private GameObject ProjectileSpawnpoint;
    private AudioSource ShootingAudioSource;
    private float LastShootTime;
    private ParticleSystem ShootSystem;
    private ObjectPool BulletPool;
    private ObjectPool<TrailRenderer> TrailPool;

    public float range = 20f;
    private GameObject closestEnemy;
    public TowerScriptableObject tower;
    private bool paused;
    public void Awake()
    {
        GameManager.instance.OnGamePaused += UpdateGamePaused;
    }
    private void UpdateGamePaused(bool isPaused)
    {
        paused = isPaused;
    }
    private void OnDestroy()
    {
        GameManager.instance.OnGamePaused -= UpdateGamePaused;
    }
    private void Update()
    {
        if(paused)
            return;
        
        Shoot();
    }

    public void Spawn(DamageConfigScriptableObject _damageConfig,
    ProjectileConfigurationScriptableObject _projectileConfig,
    TrailConfigurationScriptableObject _trailConfig,
    AuidoConfigScriptableObject _audioConfig,
    ImpactType _impactType)
    {
        DamageConfig = _damageConfig;
        ProjectileConfig = _projectileConfig;
        TrailConfig = _trailConfig;
        AudioConfig = _audioConfig;
        ImpactType = _impactType;
        
        LastShootTime = 0;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        BulletPool = ObjectPool.CreateInstance(ProjectileConfig.BulletPrefab.GetComponent<PoolableObject>(), 1);

        ProjectileSpawnpoint = gameObject.transform.Find("ProjectileSpawnpoint").gameObject;

        ShootSystem = ProjectileSpawnpoint.GetComponent<ParticleSystem>();
        ShootingAudioSource = gameObject.GetComponent<AudioSource>();
    }

    #region Shooting
    public void Shoot()
    {
        if(Time.time > ProjectileConfig.FireRate + LastShootTime)
        {
            Vector3 shootDirection;
            bool canShoot = true;
            if(closestEnemy == null || Vector3.Distance(ShootSystem.transform.position, closestEnemy.transform.position) >= range)
            {
                FindClosestEnemy(out shootDirection, out canShoot);
            }
            else
            {
                shootDirection = (closestEnemy.transform.position - ShootSystem.transform.position).normalized;
            }
            if(canShoot)
            {
                //AudioConfig.PlayShootingClip(ShootingAudioSource);
                LastShootTime = Time.time;
                ShootSystem.Play();
                                    
                if(shootDirection != Vector3.zero)
                    DoProjectileShoot(shootDirection);                
            }
        }
    }

    private void DoProjectileShoot(Vector3 ShootDirection)
    {
        Bullet bullet = Instantiate(ProjectileConfig.BulletPrefab).GetComponent<Bullet>();
        bullet.OnCollision += HandleBulletCollision;
        bullet.transform.position = ShootSystem.transform.position;
        bullet.Spawn(ProjectileConfig.BulletSpeed,closestEnemy.transform, ProjectileConfig.DamageType);
        /*TrailRenderer trail = TrailPool.Get();
        if(trail != null)
        {
            trail.transform.SetParent(bullet.transform, false);
            trail.transform.localPosition = Vector3.zero;
            trail.emitting = true;
            trail.gameObject.SetActive(true);
        }*/
    }
        /* Object Pooling Projectile
        Bullet bullet = BulletPool.GetObject().GetComponent<Bullet>();
        bullet.gameObject.SetActive(true);
        bullet.OnCollision += HandleBulletCollision;
        bullet.transform.position = ShootSystem.transform.position;
        bullet.Spawn(ProjectileConfig.BulletSpeed,closestEnemy.transform, ProjectileConfig.DamageType);*/

    private void FindClosestEnemy(out Vector3 directionToEnemy, out bool targetInRange)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float closestDistance = Mathf.Infinity;
        directionToEnemy = Vector3.zero;
        targetInRange = false;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(ShootSystem.transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance && distanceToEnemy <= range)
            {
                closestEnemy = enemy;
                closestDistance = distanceToEnemy;
                directionToEnemy = (closestEnemy.transform.position - ShootSystem.transform.position).normalized;
                targetInRange = true;
            }
        }
        
    }
    #endregion

    #region Projectile Impact
    private void HandleBulletCollision(Bullet Bullet, Collision Collision)
    {
        TrailRenderer trail = Bullet.GetComponentInChildren<TrailRenderer>();
        if(trail != null)
        {
            trail.transform.SetParent(null, true);
            StartCoroutine(DelayedDisableTrail(trail));
        }

        Bullet.gameObject.SetActive(false);
        Bullet.OnCollision -= HandleBulletCollision;

        if(Collision != null)
        {
            ContactPoint contactPoint = Collision.GetContact(0);
            
            HandleBulletImpact(
                Vector3.Distance(contactPoint.point, Bullet.SpawnLocation),
                contactPoint.point,
                contactPoint.normal,
                contactPoint.otherCollider
            );
        }
    }

    private void HandleBulletImpact(float DistanceTraveled, Vector3 HitLocation, Vector3 HitNormal, Collider HitCollider)
    {
        SurfaceManager.Instance.HandleImpact(
            HitCollider.gameObject,
            HitLocation,
            HitNormal,
            ImpactType,
            0
        );

        if(HitCollider.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(DamageConfig.GetDamage(DistanceTraveled), ProjectileConfig.DamageType);
        }
    }
    #endregion

    #region Trails
    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
    {
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = StartPoint;
        yield return null; // avoid position carry-over from last frame

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;
        while(remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                StartPoint,
                EndPoint,
                Mathf.Clamp01(1 - (remainingDistance / distance))
            );
            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }
        instance.transform.position = EndPoint;

        if(Hit.collider != null)
        {
            HandleBulletImpact(distance, EndPoint, Hit.normal, Hit.collider);
        }

        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }
    private IEnumerator DelayedDisableTrail(TrailRenderer Trail)
    {
        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        Trail.emitting = false;
        Trail.gameObject.SetActive(false);
        TrailPool.Release(Trail);
    }
    #endregion
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;


/// <summary>
/// Combines all of the Scriptable Objects necessary to create and operate a tower and handles finding and shooting at enemies.
/// </summary>
[CreateAssetMenu(fileName = "Tower", menuName = "Towers/Tower", order = 0)]
public class TowerScriptableObject : ScriptableObject, ICloneable
{
    public ImpactType ImpactType;
    public TowerType Type;
    public string Name;
    public GameObject ModelPrefab;
    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;

    public DamageConfigScriptableObject DamageConfig;
    public ProjectileConfigurationScriptableObject ProjectileConfig;
    public TrailConfigurationScriptableObject TrailConfig;
    public AuidoConfigScriptableObject AudioConfig;
    
    public ImpactType ImpactTypeOverride;
    public ICollisionHandler[] BulletImpactEffects = new ICollisionHandler[0];
    private MonoBehaviour ActiveMonoBehaviour;
    private GameObject Model;
    private GameObject ProjectileSpawnpoint;
    private AudioSource ShootingAudioSource;
    private float LastShootTime;
    private ParticleSystem ShootSystem;
    private ObjectPool BulletPool;
    private ObjectPool<TrailRenderer> TrailPool;

    private GameObject closestEnemy;
    private int enemyLayerMask;
    public GameObject SpawnModel(MonoBehaviour ActiveMonoBehaviour, Vector3 position)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        position.y = 0.2f;
        Model = Instantiate(ModelPrefab, position, Quaternion.Euler(SpawnRotation));

        return Model;
    }
    
    public void Despawn()
    {
        Destroy(Model);
        Destroy(this);
    }


    public void Spawn()
    {
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        //BulletPool = ObjectPool.CreateInstance(ProjectileConfig.BulletPrefab.GetComponent<PoolableObject>(), 1);

        ProjectileSpawnpoint = Model.transform.Find("ProjectileSpawnpoint").gameObject;
        enemyLayerMask = LayerMask.GetMask("Enemy");
        ShootSystem = ProjectileSpawnpoint.GetComponent<ParticleSystem>();
        
        ShootingAudioSource = Model.GetComponent<AudioSource>();
    }
    #region Shooting
    public void Shoot(Vector3 projectileSpawnPoint)
    {
        if(Time.time > ProjectileConfig.FireRate + LastShootTime)
        {
            Vector3 shootDirection;
            bool canShoot = true;
            if(closestEnemy == null || Vector3.Distance(ShootSystem.transform.position, closestEnemy.transform.position) >= ProjectileConfig.Range)
            {
                FindClosestEnemy(out shootDirection, out canShoot, projectileSpawnPoint);
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
        bullet.tower = this;
        bullet.OnCollision += HandleBulletCollision;
        bullet.transform.position = ShootSystem.transform.position;
        bullet.Spawn(ProjectileConfig.BulletSpeed,closestEnemy.transform);
        /* TrailRenderer trail = TrailPool.Get();
        if(trail != null)
        {
            trail.transform.SetParent(bullet.transform, false);
            trail.transform.localPosition = Vector3.zero;
            trail.emitting = true;
            trail.gameObject.SetActive(true);
        } */
    }
        /*Object Pooling Projectile
        Bullet bullet = BulletPool.GetObject().GetComponent<Bullet>();
        bullet.gameObject.SetActive(true);
        bullet.OnCollision += HandleBulletCollision;
        bullet.transform.position = ShootSystem.transform.position;
        bullet.Spawn(ProjectileConfig.BulletSpeed,closestEnemy.transform, ProjectileConfig.DamageType);*/

    private void FindClosestEnemy(out Vector3 directionToEnemy, out bool targetInRange, Vector3 origin)
    {
        directionToEnemy = Vector3.zero;
        targetInRange = false;

        float closestDistance = Mathf.Infinity;

        RaycastHit[] hits = Physics.SphereCastAll(origin, ProjectileConfig.Range, Vector3.up);

        foreach (var hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;

            // Check if the hitObject is not null
            if (hitObject != null)
            {
                if(hitObject.CompareTag("Enemy"))
                {
                    float distanceToEnemy = Vector3.Distance(origin, hitObject.transform.position);

                    if (distanceToEnemy < closestDistance)
                    {
                        closestEnemy = hitObject;
                        closestDistance = distanceToEnemy;
                        directionToEnemy = (closestEnemy.transform.position - origin).normalized;
                        targetInRange = true;
                    }
                }
            }
        }
    }

    #endregion

    #region Projectile Impact
    public void HandleBulletCollision(Bullet Bullet, Collision Collision)
    {
        TrailRenderer trail = Bullet.GetComponentInChildren<TrailRenderer>();
        if(trail != null)
        {
            trail.transform.SetParent(null, true);
            ActiveMonoBehaviour.StartCoroutine(DelayedDisableTrail(trail));
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
        foreach(ICollisionHandler handler in BulletImpactEffects)
        {
            handler.HandleImpact(HitCollider, HitLocation, HitNormal, this, ProjectileConfig.DamageType);
        }
    }
    #endregion

    #region Trails
    
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

    public object Clone()
    {
        TowerScriptableObject config = CreateInstance<TowerScriptableObject>();

        config.ImpactType = ImpactType;
        config.Type = Type;
        config.Name = Name;
        config.DamageConfig = DamageConfig.Clone() as DamageConfigScriptableObject;
        config.ProjectileConfig = ProjectileConfig.Clone() as ProjectileConfigurationScriptableObject;
        config.TrailConfig = TrailConfig.Clone() as TrailConfigurationScriptableObject;
        config.AudioConfig = AudioConfig.Clone() as AuidoConfigScriptableObject;

        config.ModelPrefab = ModelPrefab;
        config.SpawnPoint = SpawnPoint;
        config.SpawnRotation = SpawnRotation;

        config.ProjectileSpawnpoint = ProjectileSpawnpoint;
        config.ShootSystem = ShootSystem;

        config.ImpactTypeOverride = ImpactTypeOverride;

        return config;
    }
    #endregion
}


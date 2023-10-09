using UnityEngine;

/// <summary>
/// Contains the information for how a bullet should fire, what is being fired, and what the bullet can hit.
/// </summary>
[CreateAssetMenu(fileName = "Projectile Config", menuName = "Towers/Projectile Configuration", order = 2)]
public class ProjectileConfigurationScriptableObject : ScriptableObject, System.ICloneable
{
    public Bullet BulletPrefab;
    public float BulletSpeed = 1f;
    public LayerMask HitMask;
    public float FireRate = 0.25f;
    public float Range = 15f;
    public ElementType[] DamageType;

    public object Clone()
    {
        ProjectileConfigurationScriptableObject config = CreateInstance<ProjectileConfigurationScriptableObject>();
        Utilities.CopyValues(this, config);
        return config;
    }
}

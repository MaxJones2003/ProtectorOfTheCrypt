using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAreaOfEffect : ICollisionHandler
{
    public float Radius = 5;
    public AnimationCurve DamageFallOff;
    public float BaseDamage = 20;
    public int MaxEnemiesAffected = 10;

    protected Collider[] HitObjects;
    protected int Hits;

    public AbstractAreaOfEffect(float Radius, AnimationCurve DamageFallOff, float BaseDamage, int MaxEnemiesAffected)
    {
        this.Radius = Radius;
        this.DamageFallOff = DamageFallOff;
        this.BaseDamage = BaseDamage;
        this.MaxEnemiesAffected = MaxEnemiesAffected;

        HitObjects = new Collider[MaxEnemiesAffected];
    }
    public virtual void HandleImpact(Collider ImpactedObject, Vector3 HitPosition, Vector3 HitNormal, TowerScriptableObject tower, ElementType[] DamageType)
    {
        Hits = Physics.OverlapSphereNonAlloc(
            HitPosition,
            Radius,
            HitObjects,
            tower.ProjectileConfig.HitMask
        );
        for(int i = 0; i < Hits; i++)
        {
            if (HitObjects[i].TryGetComponent(out IDamageable damageable))
            {
                Debug.Log("Explosion Hit");
                float distance = Vector3.Distance(HitPosition, HitObjects[i].ClosestPoint(HitPosition));
                damageable.TakeDamage(
                    Mathf.CeilToInt(BaseDamage * DamageFallOff.Evaluate(distance / Radius)),
                    DamageType
                );
            }
        }
    }
}

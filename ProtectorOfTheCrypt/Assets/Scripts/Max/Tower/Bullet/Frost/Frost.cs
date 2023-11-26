using UnityEngine;

public class Frost : AbstractAreaOfEffect
{
    public AnimationCurve SlowDecay;

    public Frost(float Radius, AnimationCurve DamageFalloff, float BaseDamage, int MaxEnemiesAffected)
        : base(Radius, DamageFalloff, BaseDamage, MaxEnemiesAffected) { }
    public Frost(float Radius, AnimationCurve DamageFalloff, float BaseDamage, int MaxEnemiesAffected, AnimationCurve SlowDecay)
        : base(Radius, DamageFalloff, BaseDamage, MaxEnemiesAffected)
    {
        this.SlowDecay = SlowDecay;
    }

    public override void HandleImpact(Collider ImpactedObject, Vector3 HitPosition, Vector3 HitNormal, TowerScriptableObject tower, ElementType[] DamageType)
    {
        base.HandleImpact(ImpactedObject, HitPosition, HitNormal, tower, tower.ProjectileConfig.DamageType);

        for (int i = 0; i < Hits; i++)
        {
            if (HitObjects[i].TryGetComponent(out ISlowable slowable))
            {
                slowable.Slow(SlowDecay);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : AbstractAreaOfEffect
{
    public Explode(float Radius, AnimationCurve DamageFallOff, float BaseDamage, int MaxEnemiesAffected) : 
        base(Radius, DamageFallOff, BaseDamage, MaxEnemiesAffected) { }
    
}

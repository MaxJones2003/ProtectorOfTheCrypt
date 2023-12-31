using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionHandler
{
    void HandleImpact(Collider ImpactedObject, 
    Vector3 HitPosition, 
    Vector3 HitNormal, 
    TowerScriptableObject tower, 
    ElementType[] DamageType
    );
}

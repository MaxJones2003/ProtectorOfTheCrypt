using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepCollision : MonoBehaviour
{
    public ImpactType impactType;

    private void HandleImpact(Vector3 HitLocation, Vector3 HitNormal, Collider HitCollider)
    {
        SurfaceManager.Instance.HandleImpact(
            HitCollider.gameObject,
            HitLocation,
            HitNormal,
            impactType,
            0
        );
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Environment")
        {
            HandleImpact(other.GetContact(0).point, other.GetContact(0).normal, other.collider);
        }
    }
}

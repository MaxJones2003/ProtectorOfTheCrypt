using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepCollision : MonoBehaviour
{
    public ImpactType impactType;
    [SerializeField] Transform foot1, foot2;
    private float timer;
    bool useFoot1 = true;

    void Update()
    {
        // every 0.2 seconds call handle impact with the position of the footstep
        timer += Time.deltaTime;
        if (timer >= 0.2f)
        {
            timer = 0f;
            Vector3 hitLocation = useFoot1 ? foot1.position : foot2.position;
            Vector3 hitNormal = useFoot1 ? foot1.right : foot2.right;
            Collider collider = useFoot1 ? foot1.GetComponent<Collider>() : foot2.GetComponent<Collider>();
            useFoot1 = !useFoot1;
            HandleImpact(hitLocation, hitNormal, collider);
        }
    }

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
}

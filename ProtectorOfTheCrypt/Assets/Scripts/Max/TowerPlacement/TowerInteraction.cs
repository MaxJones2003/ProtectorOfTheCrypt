using UnityEngine;


public class TowerInteraction : MonoBehaviour 
{
    [SerializeField] private LayerMask placementLayerMask;
    [SerializeField] private Camera sceneCamaera;
    void Update()
    {
        // Check for click
    }


    public RaycastHit CheckTower()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamaera.nearClipPlane;
        Ray ray = sceneCamaera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            return hit;
        }
        
        return hit;
    }
}


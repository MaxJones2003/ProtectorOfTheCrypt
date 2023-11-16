using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    List<Vector3> pathPositions;
    private GameObject lookAtMe;

    private int minValue, maxValue;
    private float currentLerpValue;
    public void InitializeCameraController(List<Vector3> pathPositions)
    {
        this.pathPositions = pathPositions;
        minValue = 0;
        maxValue = this.pathPositions.Count;
    }
    // MAKE SURE THE SLIDER USES WHOLE VALUES
    public void CameraFocusLerp()
    {

    }
}

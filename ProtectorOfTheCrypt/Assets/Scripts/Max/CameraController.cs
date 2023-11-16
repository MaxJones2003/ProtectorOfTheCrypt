using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    List<Vector3> pathPositions;
    private GameObject lookAtMe;

    private int minValue, maxValue;
    private float currentLerpValue;
    Slider slider;
    //https://www.youtube.com/watch?v=nTLgzvklgU8

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(CameraFocusLerp());
    }
    public void InitializeCameraController(List<Vector3> pathPositions)
    {
        this.pathPositions = pathPositions;
        minValue = 0;
        maxValue = this.pathPositions.Count;
        // set the min an max value of the slider
        slider.minValue = minValue;
        slider.maxValue = maxValue;
    }
    
    public void CameraFocusLerp()
    {
        
    }
}

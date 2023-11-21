using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public static CameraController Instance
    {
        get
        {
            return instance;
        }
    }

    private int minValue, maxValue;
    private float currentLerpValue;
    public Slider slider;
    //https://www.youtube.com/watch?v=nTLgzvklgU8

    [Header("Runtime")]
    private GameObject lookAtObject;
    List<Vector3> pathPositions;
    CinemachineVirtualCamera virtualCamera;

    public void SetUp(List<Vector3> pathPositions)
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        this.pathPositions = pathPositions;
        lookAtObject = new GameObject();
        lookAtObject.transform.position = pathPositions[0];
        virtualCamera.LookAt = lookAtObject.transform;
        

        currentLerpValue = 0;
        slider.value = 0;
        slider.maxValue = pathPositions.Count - 1;
    }

    private void OnEnable()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        slider.onValueChanged.AddListener(CameraFocusLerp);
    }
    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(CameraFocusLerp);
    }

    public void CameraFocusLerp(float value)
    {
        lookAtObject.transform.position = pathPositions[(int)value];
    }
}

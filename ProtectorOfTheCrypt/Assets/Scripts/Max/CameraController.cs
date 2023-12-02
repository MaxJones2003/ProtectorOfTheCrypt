using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Slider lookAtSlider;
    public Slider followSlider;

    [Header("Runtime")]
    private GameObject lookAtObject;
    private GameObject followObject;
    List<Vector3> pathPositions;
    int mapWidth;
    Vector3 lowestCameraPostion, highestCameraPosition;
    CinemachineVirtualCamera virtualCamera;
    Transform pathParent;

    public void SetUp(List<Vector2Int> pathPositions2, int width, Transform pathParent)
    {
        this.pathParent = pathParent;
        List<Vector3> path = new();
        // convert the path positions to vector3s
        foreach (Vector2Int pos in pathPositions2)
        {
            path.Add(new Vector3(pos.x, 0, pos.y));
        }

        SetUp(path, width);
    }
    public void SetUp(List<Vector3> pathPositions, int width)
    {
        this.pathPositions = pathPositions;
        mapWidth = width;
        NewSetUpLookat();
        SetUpFollow();
    }

    public void ResetSliders()
    {
        lookAtSlider.value = (pathPositions.Count - 1) / 2;
        followSlider.value = 0.5f;
    }
    
    

    private void OnEnable()
    {
        lookAtSlider.onValueChanged.AddListener(NewCameraFocusLerp);
        followSlider.onValueChanged.AddListener(CameraPositionLerp);
    }
    private void OnDisable()
    {
        lookAtSlider.onValueChanged.RemoveListener(NewCameraFocusLerp);
        followSlider.onValueChanged.RemoveListener(CameraPositionLerp);
    }

    #region Lookat
    public void CameraFocusLerp(float value)
    {
        lookAtObject.transform.position = pathPositions[(int)value];
    }
    private void SetUpLookat()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        lookAtObject = new GameObject();
        lookAtObject.transform.position = FindPathAtBreadth(mapWidth/2);
        virtualCamera.LookAt = lookAtObject.transform;
        lookAtObject.name = "LookAtObject";
        lookAtObject.transform.parent = pathParent;

        lookAtSlider.maxValue = pathPositions.Count - 1;
        lookAtSlider.value = (pathPositions.Count - 1)/2;

    }
    Vector3 startLook, endLook;
    private void NewSetUpLookat()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        lookAtObject = new GameObject();
        virtualCamera.LookAt = lookAtObject.transform;
        lookAtObject.name = "LookAtObject";
        lookAtObject.transform.parent = pathParent;

        startLook = pathPositions[0];
        endLook = pathPositions[pathPositions.Count - 1];

        lookAtObject.transform.position = Vector3.Lerp(startLook, endLook, 0.5f);
        lookAtSlider.value = 0.5f;
    }
    public void NewCameraFocusLerp(float value)
    {
        // lerp between the start and end lookat positions
        lookAtObject.transform.position = Vector3.Lerp(startLook, endLook, value);
    }
    #endregion

    #region Follow
    private void SetUpFollow()
    {
        int x = mapWidth / 2, z = 0; // x is left and right, y is up and down, z is forward and backward
        int y = 12;
        followObject = new GameObject();
        followObject.transform.position = new Vector3(x, y, z);
        virtualCamera.Follow = followObject.transform;
        followObject.name = "FollowObject";
        followObject.transform.parent = pathParent;

        // get two positions, the bottom furthest back and the top furthest forward for the camera to tween between
        // use a lamda expression to find the smallest z value path
        int closestPathPointZ = LowestZValue();
        int lowZ = -5;
        int highZ = closestPathPointZ - 3;
        z = lowZ > highZ ? lowZ : highZ;
        
        lowestCameraPostion =  new Vector3(x, y, z);
        z = lowZ < highZ ? lowZ : highZ;
        highestCameraPosition = new Vector3(x, y*2, z);

        // set the value to the middle point
        followSlider.value = 0.5f;
    }
    private void CameraPositionLerp(float value)
    {
        followObject.transform.position = Vector3.Lerp(lowestCameraPostion, highestCameraPosition, value);
    }
    private int LowestZValue()
    {
        int lowestZ = 0;
        foreach (Vector3 pos in pathPositions)
        {
            if (pos.z < lowestZ)
            {
                lowestZ = (int)pos.z;
            }
        }
        return lowestZ;
    }
    private Vector3 FindPathAtDepth(int depth)
    {
        Vector3 pos = Vector3.zero;
        pos = pathPositions.Find((point) => point.z == depth);
        return pathPositions.Find((point) => point.z == depth);
    }
    private Vector3 FindPathAtBreadth(int breadth)
    {
        Vector3 pos = Vector3.zero;
        pos = pathPositions.Find((point) => point.x == breadth);
        return pathPositions.Find((point) => point.x == breadth);
    }
    #endregion
}

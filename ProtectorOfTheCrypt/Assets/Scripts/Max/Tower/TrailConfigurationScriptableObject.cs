using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Contains information for a bullet trail.
/// </summary>
[CreateAssetMenu(fileName = "Trail Config", menuName = "Towers/Projectile Trail Config", order = 4)]
public class TrailConfigurationScriptableObject : ScriptableObject , System.ICloneable
{
    public Material Material;
    public AnimationCurve WidthCurve;
    public float Duration = 0.5f;
    public float MinVertexDistance = 0.1f;
    public Gradient Color;

    public float MissDistance = 100f;
    public float SimulationSpeed = 100f;

    public object Clone()
    {
        TrailConfigurationScriptableObject config = CreateInstance<TrailConfigurationScriptableObject>();
        Utilities.CopyValues(this, config);
        return config;
    }
}


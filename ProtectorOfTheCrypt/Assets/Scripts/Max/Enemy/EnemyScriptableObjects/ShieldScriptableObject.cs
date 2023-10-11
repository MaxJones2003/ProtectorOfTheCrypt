using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each enemy should have a reference 
/// </summary>
[CreateAssetMenu(fileName = "EnemyShield", menuName = "Enemies/Shield", order = 0)]
public class ShieldScriptableObject : ScriptableObject, ICloneable
{
    [Tooltip("The physical representation of the shield")]
    public GameObject ShieldPrefab;

    public AudioClip ShieldBreakSound;
    /// <summary>
    /// The types of elements the enemy takes increased damage against
    /// </summary>
    [Tooltip("The types of elements the enemy takes increased damage against")]
    public ElementType[] Weaknesses;
    /// <summary>
    /// The types of elements the enemy takes reduced damage against
    /// </summary>
    [Tooltip("The types of elements the enemy takes reduced damage against")]
    public ElementType[] Strengths;

    public ShieldHealth Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour, EnemyHealth enemyHealth, float health)
    {

        GameObject Model = Instantiate(ShieldPrefab);
        Model.transform.position = Parent.position;
        Model.transform.parent = Parent;

        ShieldHealth shieldHealthScript = Model.AddComponent<ShieldHealth>();
        shieldHealthScript.Enable(enemyHealth, health, this, ShieldBreakSound);

        return shieldHealthScript;
    }

    public object Clone()
    {
        ShieldScriptableObject config = CreateInstance<ShieldScriptableObject>();
        
        Utilities.CopyValues(this, config);
        return config;
    }
}

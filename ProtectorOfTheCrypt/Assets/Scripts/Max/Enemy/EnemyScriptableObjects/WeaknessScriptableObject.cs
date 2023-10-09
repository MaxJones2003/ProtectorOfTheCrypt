using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each enemy should have a reference 
/// </summary>
[CreateAssetMenu(fileName = "EnemyWeakness", menuName = "Enemies/Weakness", order = 0)]
public class WeaknessScriptableObject : ScriptableObject
{
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
}

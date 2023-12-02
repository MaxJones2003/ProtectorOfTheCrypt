using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeFinder : MonoBehaviour
{
    private MageEnemyScript mageEnemyScript;
    void OnEnable()
    {
        mageEnemyScript = GetComponentInParent<MageEnemyScript>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy entered");
            mageEnemyScript.TryBoost(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy exited");
            mageEnemyScript.RemoveBoost(other.gameObject);
        }
    }
}

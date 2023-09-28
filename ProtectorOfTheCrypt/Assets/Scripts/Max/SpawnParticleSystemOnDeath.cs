using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used with the IDamageable interface if you want to spawn particles when an enemy dies.
/// </summary>
[RequireComponent(typeof(IDamageable))]
public class SpawnParticleSystemOnDeath : MonoBehaviour
{
    [SerializeField] private ParticleSystem DeathSystem;
    public IDamageable Damageable;

    private void Awake()
    {
        Damageable = GetComponent<IDamageable>();
    }

    private void OnEnable()
    {
        Damageable.OnDeath += Damageable_OnDeath;
    }

    private void OnDisable()
    {
        Damageable.OnDeath -= Damageable_OnDeath;
    }

    private void Damageable_OnDeath(Vector3 Position)
    {
        Instantiate(DeathSystem, Position, Quaternion.identity);
    }
}

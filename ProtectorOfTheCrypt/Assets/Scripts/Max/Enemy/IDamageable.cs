using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for enemies EnemyHealth script. Tells the enemy when it takes damage and when it dies.
/// </summary>
public interface IDamageable
{
    public float CurrentHealth { get; }
    public float MaxHealth { get; }

    public delegate void TakeDamageEvent(float Damage);
    public event TakeDamageEvent OnTakeDamage;

    public delegate void DeathEvent(Vector3 Position);
    public event DeathEvent OnDeath;

    public void TakeDamage(float Damage, ElementType[] DamageType);
}

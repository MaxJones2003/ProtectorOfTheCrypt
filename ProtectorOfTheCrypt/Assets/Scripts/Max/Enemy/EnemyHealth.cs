using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Interfaces with the IDamageable script, communicates with other enemy scripts about damage taken and dying.
/// </summary>
public class EnemyHealth : MonoBehaviour, IDamageable
{
    public WeaknessScriptableObject Element;
    public float _damageMultiplier = 10f;

    public float MaxHealth { get; set; }
    [SerializeField]public float CurrentHealth { get; set; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    [HideInInspector] public Spawner _spawner;

    public void Enable(float maxHealth, WeaknessScriptableObject element, float damageMultiplier, Spawner spawner) 
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        Element = element;
        _damageMultiplier = damageMultiplier;
        _spawner = spawner;
    }

    public void TakeDamage(float Damage, ElementType DamageType, float DamageModifier)
    {
        float damageTaken = Damage * CompareElementTypes(DamageType);
        damageTaken *= DamageModifier;
        // Makes sure the current health is never negative
        damageTaken = Mathf.Clamp(damageTaken, 0, CurrentHealth);
        CurrentHealth -= damageTaken;

        if(damageTaken != 0) // Damage
        {
            OnTakeDamage?.Invoke(damageTaken);
        }

        if(CurrentHealth == 0 & damageTaken != 0) // Death
        {
            _spawner.SpawnedObjects.Remove(gameObject);
            OnDeath?.Invoke(transform.position);
            Destroy(gameObject);
            if (GameManager.instance.GameMode is StoryMode)
                GameManager.instance.GameMode.CheckGameWon();
        }
    }

    private float CompareElementTypes(ElementType DamageType)
    {
        foreach(ElementType element in Element.Weaknesses) 
        {
            if(element == DamageType)
            {
                return _damageMultiplier;
            }
        }
        return 1f;
    }
}

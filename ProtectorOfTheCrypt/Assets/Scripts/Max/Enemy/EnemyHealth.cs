using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Interfaces with the IDamageable script, communicates with other enemy scripts about damage taken and dying.
/// </summary>
public class EnemyHealth : MonoBehaviour, IDamageable
{
    private List<ElementType> strengths;
    private List<ElementType> weaknesses;

    public float MaxHealth { get; set; }
    [SerializeField]public float CurrentHealth { get; set; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    [HideInInspector] public Spawner _spawner;

    private AudioClip deathSound;

    public void Enable(float maxHealth, WeaknessScriptableObject element, Spawner spawner, AudioClip audio) 
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        _spawner = spawner;
        deathSound = audio;
        strengths = new();
        weaknesses = new();
        foreach(ElementType strength in element.Strengths)
            strengths.Add(strength);
        foreach(ElementType weakness in element.Weaknesses)
            weaknesses.Add(weakness);
    }

    public void OnDestroy()
    {
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, deathSound);
    }

    public void TakeDamage(float Damage, ElementType[] DamageType)
    {
        float damageTaken = Damage * CompareElementTypes(DamageType);
        // Makes sure the current health is never negative
        damageTaken = Mathf.Clamp(damageTaken, 0, CurrentHealth);
        CurrentHealth -= damageTaken;

        if(damageTaken != 0) // Damage
        {
            OnTakeDamage?.Invoke(damageTaken);
        }

        if (CurrentHealth == 0 & damageTaken != 0) // Death
        {
            _spawner.SpawnedObjects.Remove(gameObject);
            OnDeath?.Invoke(transform.position);
            if (GameManager.instance.GameMode is StoryMode)
            {
                if (GameManager.instance.GameMode.CheckGameWon())
                        GameManager.instance.GameMode.OnGameWon();
            }
            GameManager.instance.RemoveMoney(-5);
            Destroy(gameObject);
        }
    }

    private float CompareElementTypes(ElementType[] DamageType)
    {
        float damageMultiplier = 1f;
        foreach(ElementType element in DamageType) 
        {
            if(weaknesses.Contains(element))
            {
                damageMultiplier *= 2f;
                damageMultiplier = Mathf.Min(damageMultiplier, 4);
            }
            else if(strengths.Contains(element))
            {
                damageMultiplier /= 2f;
                damageMultiplier = Mathf.Max(damageMultiplier, 0.25f);
            }
        }
        return damageMultiplier;
    }
}

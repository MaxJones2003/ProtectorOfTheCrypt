using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interfaces with the IDamageable script, communicates with other enemy scripts about damage taken and dying.
/// </summary>
public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float MaxHealth { get; set; }
    [SerializeField]public float CurrentHealth { get; set; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    [HideInInspector] public Spawner _spawner;

    private AudioClip deathSound;

    private ShieldHealth shieldScript;

    public void Enable(float maxHealth, Spawner spawner, AudioClip audio,ShieldScriptableObject shield) 
    {
        gameObject.GetComponent<Collider>().enabled = false;
        StartCoroutine(EnableCollider());
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        _spawner = spawner;
        deathSound = audio;
        
        // Handle Shield Setup
        if(shield != null) shieldScript = shield.Spawn(transform, this, this);
    }

    public void OnDestroy()
    {
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, deathSound);
    }

    public void TakeDamage(float Damage, ElementType[] DamageType)
    {
        float damageTaken = Damage;

        // Check if enemy has a shield
        // If enemy has a shield, call TakeDamage() on the shield.
        // If the shield is boken its TakeDamage function returns the left over damage which gets dealt to the enemy
        if(shieldScript != null)
        {
            Debug.Log(damageTaken);
            damageTaken = shieldScript.TakeDamage(Damage, DamageType);
            Debug.Log(damageTaken);
        }

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

    public void BreakShield()
    {
        shieldScript = null;
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<Collider>().enabled = true;
    }
    
}

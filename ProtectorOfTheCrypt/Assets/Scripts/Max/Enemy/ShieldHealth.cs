using System.Collections.Generic;
using UnityEngine;

public class ShieldHealth : MonoBehaviour
{
    EnemyHealth Enemy;
    private List<ElementType> strengths;
    private List<ElementType> weaknesses;

    public float MaxHealth { get; set; }
    [SerializeField]public float CurrentHealth { get; set; }

    private AudioClip shieldBreakSound;

    public void Enable(EnemyHealth enemy, float maxHealth, ShieldScriptableObject shield, AudioClip audio) 
    {
        Enemy = enemy;
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        shieldBreakSound = audio;
        strengths = new();
        weaknesses = new();
        foreach(ElementType strength in shield.Strengths)
            strengths.Add(strength);
        foreach(ElementType weakness in shield.Weaknesses)
            weaknesses.Add(weakness);
    }

    public void OnDestroy()
    {
        if(shieldBreakSound != null)
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, shieldBreakSound);
    }

    public float TakeDamage(float Damage, ElementType[] DamageType)
    {
        float damageTaken = Damage * CompareElementTypes(DamageType);
        // Makes sure the current health is never negative
        CurrentHealth -= damageTaken;

        if (CurrentHealth <= 0 & damageTaken != 0) // Death
        {
            if (GameManager.instance.GameMode is StoryMode)
            {
                if (GameManager.instance.GameMode.CheckGameWon())
                        GameManager.instance.GameMode.OnGameWon();
            }
            Enemy.BreakShield();
            Destroy(gameObject);
            return damageTaken;
        }
        return 0f;
        
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
   [SerializeField] private float maxHealth = 100f;
   [SerializeField] private float currentHealth = 0f;

   public UnityEvent<float> OnHealthChanged;
   public UnityEvent OnDeath;

   private bool isDead = false;

   private void Awake()
   {
     currentHealth = maxHealth;
   }

   public void TakeDamage(float damageAmount)
   {
     if (isDead) return;

     currentHealth -= damageAmount;
     currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

     OnHealthChanged?.Invoke(currentHealth / maxHealth);

     if (currentHealth <= 0f)
     {
        Die();
     }
   }

   public void Heal(float healAmount)
   {
    if (isDead) return;

    currentHealth += healAmount;
    currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

    OnHealthChanged?.Invoke(currentHealth / maxHealth);
   }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();

        // Can add additional death logic here, such as:
        // - Playing death animation
        // - Spawning loot
        // - Removing the game object
        // Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }
}

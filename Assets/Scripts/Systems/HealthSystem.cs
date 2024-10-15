using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
  [SerializeField] private float maxHealth = 100f;
  [SerializeField] private float currentHealth = 0f;

  [SerializeField] private float regenAmount = 2f;

  [SerializeField] private float regenCooldown = 5f;

  public UnityEvent<float> OnHealthChanged;
  public UnityEvent OnDeath;

  private bool isDead = false;

  private void Awake()
  {
    currentHealth = maxHealth;
  }

  public void TakeDamage(float damageAmount)
  {
    // Prevent taking damage if the object is already dead
    if (isDead) return;

    // Subtract the damage amount from the current health amount
    currentHealth -= damageAmount;

    // Clamp the current health to ensure it is not less than 0f or more than the maxHealth
    currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

    // Raise the OnHealthChanged UnityEvent with the current health percentage
    OnHealthChanged?.Invoke(currentHealth / maxHealth);

    // If the current health is 0f or less, die
    if (currentHealth <= 0f)
    {
      // Die and raise the OnDeath UnityEvent
      Die();
    }
    else
    {
      regenCoroutine = StartCoroutine(RegenHealth());
    }
  }



  public void Heal(float healAmount)
  {
    // Prevent healing if the object is already dead
    if (isDead) return;

    // Add the heal amount to the current health amount
    currentHealth += healAmount;

    // Ensure the current health is not more than the maxHealth
    currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

    // Raise the OnHealthChanged UnityEvent with the current health percentage
    OnHealthChanged?.Invoke(currentHealth / maxHealth);
  }

  private ienumerator RegenHealth()
  {
    while (Time.time - lastDamageTime < regenCooldown)
    {
      yield return null;
    }

    while (currentHealth < maxHealth && !isDead)
    {
      currentHealth += regenAmount * Time.deltaTime;
      currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
      OnHealthChanged?.Invoke(currentHealth / maxHealth);

      yield return null;
    }
    regenCoroutine  = null;
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

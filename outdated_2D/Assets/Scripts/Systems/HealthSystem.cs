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
   private float lastDamageTime = 0f;
  private Coroutine regenCoroutine;

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

  // This coroutine handles the regeneration of health over time
  private IEnumerator RegenHealth()
  {
    // Wait until the regen cooldown period has passed since the last damage taken
    while (Time.time - lastDamageTime < regenCooldown)
    {
      yield return null;
    }

    // Continue regenerating health as long as the current health is below the maximum and the object is not dead
    while (currentHealth < maxHealth && !isDead)
    {
      // Increase the current health based on the regen amount over time
      currentHealth += regenAmount * Time.deltaTime;
      
      // Ensure the current health does not exceed the maximum health
      currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
      
      // Update the health change event with the current health percentage
      OnHealthChanged?.Invoke(currentHealth / maxHealth);

      yield return null;
    }

    // Reset the regenCoroutine when the regeneration is complete
    regenCoroutine = null;
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

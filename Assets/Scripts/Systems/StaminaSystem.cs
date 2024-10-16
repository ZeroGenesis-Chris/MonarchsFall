using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
 [SerializeField] private float maxStamina = 100f;
 [SerializeField] private float staminaRegenRate = 5f;
 [SerializeField] private float staminaRegenDelay = 1f;

 private float currentStamina;
 private bool isRegenerating = false;
 private Coroutine regenCoroutine;

 void Start()
 {
    currentStamina = maxStamina;
 }

 /// <summary>
 /// Uses the given amount of stamina, or returns false if not enough is available.
 /// If the stamina is used, a coroutine is started to regenerate the stamina after a delay.
 /// </summary>
 /// <param name="amount">The amount of stamina to use.</param>
 /// <returns>True if the stamina was used, false if not enough is available.</returns>
 public bool UseStamina(float amount)
 {
    // If the current stamina is greater than or equal to the given amount
    if (currentStamina >= amount)
    {
        // Subtract the given amount from the current stamina
        currentStamina -= amount;

        // If there is already a regen coroutine running, stop it
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }

        // Start a coroutine to regenerate the stamina after a delay
        regenCoroutine = StartCoroutine(RegenStaminaAfterDelay());

        // Return true to indicate that the stamina was used
        return true;
    }

    // Return false to indicate that the stamina was not used
    return false;
 }

    /// <summary>
    /// Waits for a delay, then regenerates the stamina over time until it reaches the maximum.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RegenStaminaAfterDelay()
    {
        // Wait for the given delay before regenerating the stamina
        yield return new WaitForSeconds(staminaRegenDelay);

        // Set a flag to indicate that regeneration is happening
        isRegenerating = true;

        // While the current stamina is less than the maximum and regeneration is happening
        while (currentStamina < maxStamina && isRegenerating)
        {
            // Increase the current stamina by the regeneration rate multiplied by the time since the last frame
            currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);

            // Wait until the next frame before increasing the stamina again
            yield return null;
        }

        // Set the flag to false to indicate that regeneration is no longer happening
        isRegenerating = false;
    }

    /// <summary>
    /// Stops any current stamina regeneration coroutine and sets a flag to indicate that regeneration is no longer happening.
    /// </summary>
    private void StopRegen()
    {
        // If there is currently a coroutine running to regenerate stamina
        if (regenCoroutine != null)
        {
            // Stop the coroutine
            StopCoroutine(regenCoroutine);
        }

        // Set a flag to indicate that regeneration is no longer happening
        isRegenerating = false;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public float GetMaxStamina()
    {
        return maxStamina;
    }
}

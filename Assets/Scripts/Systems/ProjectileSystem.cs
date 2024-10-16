using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ProjectileSystem : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    public float damage = 10f;

    private Vector2 direction;
    private ChargeLevel chargeLevel;

    public void Initialize(Vector2 targetPosition, ChargeLevel level)
    {
        direction = (targetPosition - (Vector2)transform.position).normalized;
        chargeLevel = level;

        // Adjust the properties of the projectile
        AdjustProjectileProperties();

        // Destroy the projectile after the lifetime
        Destroy(gameObject, lifetime);

    }

    private void Update()
    {
        //Move Projectile
        transform.Translate(speed * Time.deltaTime * direction);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthSystem healthSystem = other.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            // Deal damage to the target
            healthSystem.TakeDamage(damage);
            // Destroy the projectile
            Destroy(gameObject);
        }
    }
    private void AdjustProjectileProperties()
    {
        switch (chargeLevel)
        {
            case ChargeLevel.None:
                // No adjustment for uncharged projectiles
                break;
            case ChargeLevel.Low:
                damage *= 1.2f;
                speed *= 1.1f;
                break;
            case ChargeLevel.Medium:
                damage *= 1.5f;
                speed *= 1.2f;
                break;
            case ChargeLevel.High:
                damage *= 1.8f;
                speed *= 1.3f;
                break;
            case ChargeLevel.Critical:
                damage *= 2f;
                speed *= 1.5f;
                break;
        }
    }
}

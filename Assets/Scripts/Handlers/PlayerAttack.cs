using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Melee, Ranged }
public enum ChargeLevel { None, Low, Medium, High, Critical }

public class PlayerAttack : MonoBehaviour
{
    public WeaponType equippedWeapon;
    public int meleeSwingCount = 3; // Number of swings before critical
    private int currentSwingCount = 0;
    private float chargeTime = 0f; // Charge duration for range attacks
    public float maxChargeTime = 2f; // Max hold time for critical

    //Charge shot window
    public float lowChargeTime = 0.5f;
    public float mediumChargeTime = 1f;
    public float highChargeTime = 1.5f;

    //Systems Import
    private DamageSystem damageSystem;
    private MeleeTargetingSystem meleeTargetingSystem;
    private RangeTargetSystem rangedTargetingSystem;

    private GameObject projectilePrefab;

    void Start()
    {
        damageSystem = GetComponent<DamageSystem>();
        meleeTargetingSystem = GetComponent<MeleeTargetingSystem>();
        rangedTargetingSystem = GetComponent<RangeTargetSystem>();
    }

    void Update()
    {
        // Check if the "Fire1" button has been pressed this frame
        if (Input.GetButtonDown("Fire1"))
        {
            // If the equipped weapon is Melee, perform a melee attack
            if (equippedWeapon == WeaponType.Melee)
            {
                MeleeAttack();
            }
            // If the equipped weapon is Ranged, start charging a ranged attack
            else if (equippedWeapon == WeaponType.Ranged)
            {
                StartChargingRangedAttack();
            }
        }

        // Check if the equipped weapon is Ranged and the "Fire1" button has been released
        if (equippedWeapon == WeaponType.Ranged && Input.GetButtonUp("Fire1"))
        {
            // Release the ranged attack with the current charge level
            ReleaseRangedAttack();
        }
    }


    void MeleeAttack()
    {
        // Increment the current swing count
        currentSwingCount++;

        // If the current swing count is less than the total number of swings before a critical hit
        if (currentSwingCount < meleeSwingCount)
        {
            // Perform a normal melee attack
            PerformMeleeSwing(false);
        }
        else
        {
            // Perform a critical melee attack
            PerformMeleeSwing(true);

            // Reset the swing count
            currentSwingCount = 0;
        }
    }


    void PerformMeleeSwing(bool isCritical)
    {
        // Get a target from the melee targeting system
        GameObject target = meleeTargetingSystem.GetMeleeTarget();

        // If a target is found
        if (target != null)
        {
            // Calculate the damage multiplier for the attack
            // If the attack is a critical hit, the damage multiplier is 2
            // Otherwise, the damage multiplier is 1
            float damageMultiplier = isCritical ? 2f : 1f;

            // Deal damage to the target using the damage system
            // The weapon type is Melee, the attack is critical if isCritical is true,
            // the target is the target we just found, and the damage multiplier is the value we calculated
            damageSystem.DealDamage(WeaponType.Melee, isCritical, target, damageMultiplier);
        }
    }


    void StartChargingRangedAttack()
    {
        chargeTime = 0f;
        StartCoroutine(ChargeRangedAttack());
    }

    IEnumerator ChargeRangedAttack()
    {
        while (Input.GetButton("Fire1")) // While button is held down
        {
            chargeTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
    }

    void ReleaseRangedAttack()
    {
        ChargeLevel chargeLevel = GetChargeLevel();
        PerformRangedAttack(chargeLevel);
        chargeTime = 0f; // Reset charge time
    }

    void PerformRangedAttack(ChargeLevel chargeLevel)
    {
        // Get the target of the ranged attack
        GameObject target = rangedTargetingSystem.GetRangedTarget();
        if (target != null)
        {
            // Get the health system of the target
            HealthSystem targetHealth = target.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                // Instantiate the ranged attack prefab
                GameObject projectileObject = Instantiate(projectilePrefab,transform.position,Quaternion.identity);
                ProjectileSystem projectile = projectileObject.GetComponent<ProjectileSystem>();

                // Initialize the projectile
                projectile.Initialize(target.transform.position, chargeLevel);

                // Deal damage to the target
                float damageMultiplier = GetDamageMultiplier(chargeLevel);
                damageSystem.DealDamage(WeaponType.Ranged, chargeLevel == ChargeLevel.Critical, target, damageMultiplier);
            }
        }
    }

    ChargeLevel GetChargeLevel()
    {
        if (chargeTime < lowChargeTime)
            return ChargeLevel.None;
        else if (chargeTime < mediumChargeTime)
            return ChargeLevel.Low;
        else if (chargeTime < highChargeTime)
            return ChargeLevel.Medium;
        else if (chargeTime < maxChargeTime)
            return ChargeLevel.High;
        else
            return ChargeLevel.Critical;
    }

    float GetDamageMultiplier(ChargeLevel chargeLevel)
    {
        switch (chargeLevel)
        {
            case ChargeLevel.None:
                return 1f;
            case ChargeLevel.Low:
                return 1.2f;
            case ChargeLevel.Medium:
                return 1.5f;
            case ChargeLevel.High:
                return 1.8f;
            case ChargeLevel.Critical:
                return 2f;
            default:
                return 1f;
        }
    }
}
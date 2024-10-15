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
    private HealthSystem Health;
    private MeleeTargetingSystem meleeTargetingSystem;
    private RangeTargetSystem rangedTargetingSystem;

    void Start()
    {
        damageSystem = GetComponent<DamageSystem>();
        meleeTargetingSystem = GetComponent<MeleeTargetingSystem>();
        rangedTargetingSystem = GetComponent<RangeTargetSystem>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (equippedWeapon == WeaponType.Melee)
            {
                MeleeAttack();
            }
            else if (equippedWeapon == WeaponType.Ranged)
            {
                StartChargingRangedAttack();
            }
        }
        if (equippedWeapon == WeaponType.Ranged && Input.GetButtonUp("Fire1"))
        {
            ReleaseRangedAttack();
        }
    }

    void MeleeAttack()
    {
        currentSwingCount++;

        if (currentSwingCount < meleeSwingCount)
        {
            PerformMeleeSwing(false);
        }
        else
        {
            PerformMeleeSwing(true);
            currentSwingCount = 0; // Reset swing count
        }
    }

    void PerformMeleeSwing(bool isCritical)
    {
        GameObject target = meleeTargetingSystem.GetMeleeTarget();
        if (target != null)
        {
            float damageMultiplier = isCritical ? 2f : 1f;
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

   void PerformRangedAttack(ChargeLevel chargeLevel)
    {
        GameObject target = rangedTargetingSystem.GetRangedTarget();
        if (target != null)
        {
            HealthSystem targetHealth = target.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                float damageMultiplier = GetDamageMultiplier(chargeLevel);
                damageSystem.DealDamage(WeaponType.Ranged, chargeLevel == ChargeLevel.Critical, target, damageMultiplier);
                
                // Instantiate projectile with charge level
                // GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                // projectile.GetComponent<Projectile>().Initialize(target.transform.position, chargeLevel);
            }
        }
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
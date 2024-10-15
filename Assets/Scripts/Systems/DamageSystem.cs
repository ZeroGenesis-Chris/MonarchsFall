using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [System.Serializable]
    public class WeaponDamage
    {
        public float normalDamage;
        public float criticalDamage;
    }

    public WeaponDamage meleeDamage;
    public WeaponDamage rangedDamage;

    // This function deals damage to a target based on the weapon type, critical hit status, target object, and damage multiplier
    public void DealDamage(WeaponType weaponType, bool isCritical, GameObject target, float damageMultiplier = 1f)
    {
        // Calculate the base damage based on the weapon type and critical hit status
        float baseDamage = CalculateDamage(weaponType, isCritical);
        
        // Calculate the final damage by multiplying the base damage with the damage multiplier
        float finalDamage = baseDamage * damageMultiplier;

        // Get the HealthSystem component of the target object
        HealthSystem targetHealth = target.GetComponent<HealthSystem>();

        // If the target has a HealthSystem component, apply the final damage to the target
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(finalDamage);
        }
    }


    private float CalculateDamage(WeaponType weaponType, bool isCritical)
    {
        // Get the appropriate WeaponDamage struct for the given weapon type
        WeaponDamage damage = (weaponType == WeaponType.Melee) ? meleeDamage : rangedDamage;

        // If the attack is a critical hit, return the critical damage value
        // Otherwise, return the normal damage value
        return isCritical ? damage.criticalDamage : damage.normalDamage;
    }

}
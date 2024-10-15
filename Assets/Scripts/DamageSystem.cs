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

    public void DealDamage(WeaponType weaponType, bool isCritical, GameObject target, float damageMultiplier = 1f)
    {
        float baseDamage = CalculateDamage(weaponType, isCritical);
        float finalDamage = baseDamage * damageMultiplier;

        HealthSystem targetHealth = target.GetComponent<HealthSystem>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(finalDamage);
        }
    }

    private float CalculateDamage(WeaponType weaponType, bool isCritical)
    {
        WeaponDamage damage = (weaponType == WeaponType.Melee) ? meleeDamage : rangedDamage;
        return isCritical ? damage.criticalDamage : damage.normalDamage;
    }
}
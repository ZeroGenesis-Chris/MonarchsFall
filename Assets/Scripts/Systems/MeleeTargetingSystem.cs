using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeTargetingSystem : MonoBehaviour
{
    public float MeleeRange = 2f;
    public float meleeAngle = 60f;
    public LayerMask targetLayer;

    /// <summary>
    /// Gets the best melee target based on the following criteria:
    /// 1. Distance: The target must be within the melee range.
    /// 2. Angle: The target must be within the melee angle (meleeAngle) of the player's right direction.
    /// </summary>
    public GameObject GetMeleeTarget()
    {
        // Get all colliders within the melee range
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, MeleeRange, targetLayer);

        // Initialize the best target as null and the closest angle as the melee angle
        GameObject bestTarget = null;
        float closestAngle = meleeAngle;

        // Loop through all potential targets
        foreach (Collider2D collider in potentialTargets)
        {
            // Calculate the direction vector from the player to the target
            Vector2 directionToTarget = collider.transform.position - transform.position;

            // Calculate the angle between the direction to the target and the player's right direction
            float angle = Vector2.Angle(transform.right, directionToTarget);

            // Check if the angle is less than the closest angle
            if (angle < closestAngle)
            {
                // Update the closest angle and the best target
                closestAngle = angle;
                bestTarget = collider.gameObject;
            }
        }

        // Return the best target
        return bestTarget;
    }

    //Visualization for debugging
    private void OnDrawGizmosSelected()
    {
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, MeleeRange);

            Vector2 rightDir = Quaternion.Euler(0, 0, meleeAngle * 0.5f) * transform.right;
            Vector2 leftDir = Quaternion.Euler(0, 0, -meleeAngle * 0.5f) * transform.right;
            Gizmos.DrawRay(transform.position, rightDir * MeleeRange);
            Gizmos.DrawRay(transform.position, leftDir * MeleeRange);

        }
    }
}

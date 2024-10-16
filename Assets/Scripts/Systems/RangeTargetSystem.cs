using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangeTargetSystem : MonoBehaviour
{
    public float maxRange = 10;
    public float fieldOfView = 90f;
    public LayerMask targetLayers;
    public bool requireLineOfSight = true;

    public GameObject GetRangedTarget()
    {
        // Find all colliders in the range of the attack
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, maxRange, targetLayers);

        // Initialize the best target as null and the closest distance as infinity
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        // Loop through all potential targets
        foreach (Collider2D collider in potentialTargets)
        {
            // Calculate the direction to the target
            Vector2 directionToTarget = collider.transform.position - transform.position;

            // Calculate the angle between the direction to the target and the right direction
            float angle = Vector2.Angle(transform.right, directionToTarget);

            // If the angle is less than half the field of view, then the target is in our sights
            if (angle < fieldOfView * 0.5f)
            {
                // Calculate the distance squared to the target
                float distanceSqr = directionToTarget.sqrMagnitude;

                // If the distance is less than the closest distance, then this target is now the best target
                if (distanceSqr < closestDistanceSqr)
                {
                    // If line of sight is required, check if there is a line of sight to the target
                    if (!requireLineOfSight || HasLineOfSight(collider.transform.position))
                    {
                        // Update the best target
                        bestTarget = collider.gameObject;
                        closestDistanceSqr = distanceSqr;
                    }
                }
            }
        }

        // Return the best target
        return bestTarget;
    }
    private bool HasLineOfSight(Vector2 targetPosition)
    {
        // Calculate the direction vector from the current object to the target position
        Vector2 direction = targetPosition - (Vector2)transform.position;

        // Cast a ray from the current position towards the target position
        // The ray will check for collisions within the specified maxRange and targetLayers
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, direction, maxRange, targetLayers);

        // Check if the ray hit an object and if that object's position matches the target position
        // If both conditions are true, there is a line of sight to the target
        return hit.collider != null && (Vector2)hit.transform.position == targetPosition;
    }
    
    //Visualization for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxRange);

        Vector2 rightDir = Quaternion.Euler(0, 0, fieldOfView * 0.5f) * transform.right;
        Vector2 leftDir = Quaternion.Euler(0, 0, -fieldOfView * 0.5f) * transform.right;
        Gizmos.DrawRay(transform.position, rightDir * maxRange);
        Gizmos.DrawRay(transform.position, leftDir * maxRange);
    }
}


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
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, maxRange, targetLayers);

        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Collider2D collider in potentialTargets)
        {
            Vector2 directionToTarget = collider.transform.position - transform.position;
            float angle = Vector2.Angle(transform.right, directionToTarget);

            if (angle < fieldOfView * 0.5f)
            {
                float distanceSqr = directionToTarget.sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    if (!requireLineOfSight || HasLineOfSight(collider.transform.position))
                    {

                    }
                }
            }
        }

        return bestTarget;
    }
    private bool HasLineOfSight(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, direction, maxRange, targetLayers);

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


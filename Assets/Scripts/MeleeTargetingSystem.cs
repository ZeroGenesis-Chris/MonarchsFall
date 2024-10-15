using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeTargetingSystem : MonoBehaviour
{
    public float MeleeRange = 2f;
    public float meleeAngle = 60f;
    public LayerMask targetLayer;

    public GameObject GetMeleeTarget()
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, MeleeRange, targetLayer);

        GameObject bestTarget = null;
        float closestAngle = meleeAngle;

        foreach (Collider2D collider in potentialTargets)
        {
            Vector2 directionToTarget = collider.transform.position - transform.position;
            float angle = Vector2.Angle(transform.right, directionToTarget);

            if (angle < closestAngle)
            {
                closestAngle = angle;
                bestTarget = collider.gameObject;
            }
        }

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

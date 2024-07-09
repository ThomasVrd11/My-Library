/*
 * ======================================================================================
 *                            WeaponDamage Script
 * ======================================================================================
 * This script handles the weapon damage mechanics for the player. It performs a sweep 
 * attack, calculating damage based on the player's current animation state and applies 
 * the damage to any enemies hit within the attack range.
 *
 * Key Features:
 * - Performs a sweep attack to hit multiple enemies.
 * - Calculates damage based on the current animation state.
 * - Provides debug information for the attack.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    // * Reference to the player's animator
    [SerializeField] Animator playerAnimator;

    // * Base damage of the weapon
    [SerializeField] int baseDamage = 10;

    // * Layer mask to identify enemy objects
    [SerializeField] LayerMask enemyLayer;

    // * Range and radius of the attack
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackRadius = 0.5f;

    // * Transform representing the attack point
    [SerializeField] Transform attackPoint;

    // * Debug mode flag
    public bool debugMode;

    private void PerformSweepAttack()
    {
        // * Calculate the number of steps in the sweep attack
        int steps = Mathf.CeilToInt(attackRange / attackRadius);

        // * Perform the sweep attack by checking for enemies in each step
        for (int i = 0; i <= steps; i++)
        {
            Vector3 sweepPosition = attackPoint.position + attackPoint.forward * (i * attackRadius);
            RaycastHit[] hits = Physics.SphereCastAll(sweepPosition, attackRadius, attackPoint.forward, 0.1f, enemyLayer);

            foreach (RaycastHit hit in hits)
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int weaponDamage = CalculateDamage();
                    enemy.TakeDamage(weaponDamage);
                    if (debugMode) Debug.Log("Dealt " + weaponDamage + " damage to " + hit.collider.name);
                }
            }
        }
    }

    private int CalculateDamage()
    {
        // * Calculate the damage based on the player's current animation state
        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(1);
        if (stateInfo.IsName("Skill1Stage1")) return baseDamage;
        if (stateInfo.IsName("Skill1Stage2")) return Mathf.RoundToInt(baseDamage * 1.5f);
        if (stateInfo.IsName("Skill1Stage3")) return Mathf.RoundToInt(baseDamage * 2.5f);
        return baseDamage;
    }

    public void OnAttack()
    {
        // * Perform the attack and log debug information if in debug mode
        if (debugMode) Debug.Log("Performing sweep");
        PerformSweepAttack();
    }

    private void OnDrawGizmos()
    {
        // * Draw gizmos to visualize the attack range and radius in the editor
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
            int steps = Mathf.CeilToInt(attackRange / attackRadius);
            for (int i = 0; i <= steps; i++)
            {
                Vector3 sweepPosition = attackPoint.position + attackPoint.forward * (i * attackRadius);
                Gizmos.DrawWireSphere(sweepPosition, attackRadius);
                if (i < steps)
                {
                    Vector3 nextSweepPosition = attackPoint.position + attackPoint.forward * ((i + 1) * attackRadius);
                    Gizmos.DrawLine(sweepPosition, nextSweepPosition);
                }
            }
        }
    }
}

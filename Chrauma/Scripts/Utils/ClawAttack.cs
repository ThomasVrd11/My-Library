/*
 * ======================================================================================
 *                              ClawAttack Script
 * ======================================================================================
 * This script handles the claw attack behavior for an enemy. It applies damage to the 
 * player on collision and includes a cooldown to prevent repeated damage in quick succession.
 *
 * Key Features:
 * - Detects collision with the player.
 * - Applies damage to the player and starts a cooldown.
 * - Prevents repeated damage during the cooldown period.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClawAttack : MonoBehaviour
{
    // * Amount of damage dealt by the claw attack
    [SerializeField] int damage;
    
    // * Cooldown flag to prevent repeated damage
    private bool damageCD = false;

    private void Start()
    {
        // * Initialization if needed
    }

    private void OnTriggerEnter(Collider other)
    {
        // * Apply damage to the player on collision if not in cooldown
        if (other.name == "Player" && !damageCD)
        {
            PlayerStats.instance.TakeDamage(damage);
            damageCD = true;
            StartCoroutine(CooldownAttack());
        }
    }

    private IEnumerator CooldownAttack()
    {
        // * Wait for 2 seconds before allowing damage again
        yield return new WaitForSeconds(2);
        damageCD = false;
    }
}

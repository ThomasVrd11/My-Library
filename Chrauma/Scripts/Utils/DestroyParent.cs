/*
 * ======================================================================================
 *                             DestroyParent Script
 * ======================================================================================
 * This script destroys the parent GameObject when the attached GameObject collides with
 * another GameObject tagged as "Player".
 *
 * Key Features:
 * - Detects collision with objects tagged as "Player".
 * - Destroys the parent GameObject upon collision.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParent : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // * Check if the colliding object is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // * Destroy the parent GameObject
            Destroy(transform.parent.gameObject);
        }
    }
}

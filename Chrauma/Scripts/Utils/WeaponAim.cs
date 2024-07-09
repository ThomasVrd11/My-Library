/*
 * ======================================================================================
 *                            weaponAim Script
 * ======================================================================================
 * This script handles the aiming of a weapon based on the player's mouse position.
 * The weapon will rotate to face the direction of the mouse cursor in world space.
 *
 * Key Features:
 * - Rotates the weapon to face the mouse cursor.
 * - Calculates the mouse position in world space.
 * - Ensures the weapon rotates only along the Y axis.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponAim : MonoBehaviour
{
    // * Reference to the player transform
    [SerializeField] public Transform player;
    private Transform weaponSlot;

    private void Awake()
    {
        // * Initialize the weaponSlot transform
        weaponSlot = this.gameObject.transform;
    }

    void Update()
    {
        // * Get the mouse position in world space
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3 playerToMouseDir = mouseWorldPosition - player.position;

        // * Convert the direction to a global rotation
        Quaternion lookRotation = Quaternion.LookRotation(playerToMouseDir);

        // * Set the weapon slot's global rotation
        // * Ensure we only modify the Y rotation
        weaponSlot.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
    }

    Vector3 GetMouseWorldPosition()
    {
        // * Calculate the mouse position in world space
        Plane plane = new Plane(Vector3.up, player.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }
}

/*
 * ======================================================================================
 *                          FollowLifeDrop Script
 * ======================================================================================
 * This script handles the behavior of life drop items that follow a target (e.g., player).
 * It uses SmoothDamp for smooth movement towards the target and heals the player upon
 * reaching the target.
 *
 * Key Features:
 * - Initiates following behavior on command.
 * - Smoothly moves towards the target using SmoothDamp.
 * - Heals the player upon collision with the target.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLifeDrop : MonoBehaviour
{
    // * The target that the life drop will follow
    public Transform Target;

    // * Minimum and maximum modifiers for the smooth damp function
    public float MinModifier = 7;
    public float MaxModifier = 10;

    // * Internal variables for smooth movement
    Vector3 _velocity = Vector3.zero;
    bool _isFollowing = false;

    // * Start following the target
    public void StartFollowing()
    {
        _isFollowing = true;
    }

    void Update()
    {
        // * If following is enabled, move towards the target smoothly
        if (_isFollowing)
        {
            transform.position = Vector3.SmoothDamp(transform.position, Target.position, ref _velocity, Time.deltaTime * Random.Range(MinModifier, MaxModifier));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // * If the life drop reaches the target, heal the player and destroy the life drop
        if (other.gameObject.tag == "LifeDropTarget")
        {
            PlayerStats.instance.Heal(5);
            Destroy(gameObject);
        }
    }
}

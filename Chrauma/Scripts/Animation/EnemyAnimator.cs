/*
 * ======================================================================================
 *                                 EnemyAnimator Script
 * ======================================================================================
 * This script handles the animation states of an enemy character in Unity.
 * It checks the movement of the enemy and switches between walking and idle animations.
 * Additionally, it triggers an attack animation when the enemy attacks.
 *
 * Key Features:
 * - Monitors enemy movement to switch between walking and idle animations.
 * - Triggers attack animation.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;
    private Transform parent;
    private Vector3 oldPosition;
    private bool isWalking = false;

    void Start()
    {
        // * Initialize animator and set the parent transform and old position
        animator = GetComponent<Animator>();
        parent = gameObject.transform.parent;
        oldPosition = parent.position;
        // * The Rigidbody component is commented out and not used
        // * rb = parent.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // * Check if the parent object has moved since the last frame
        if (parent.position != oldPosition)
        {
            isWalking = true;
            oldPosition = parent.position;
        }
        else
        {
            isWalking = false;
        }

        // * Set the animator parameters based on movement state
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isIdle", !isWalking);
    }

    public void startAttackAnimation()
    {
        // * Trigger the attack animation
        animator.SetTrigger("isAttacking");
    }
}

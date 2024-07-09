/*
 * ======================================================================================
 *                          SmoothCameraControl Script
 * ======================================================================================
 * This script provides smooth camera following behavior. The camera smoothly follows
 * a target object with a specified offset and smoothing time.
 *
 * Key Features:
 * - Calculates the initial offset between the camera and the target.
 * - Smoothly updates the camera's position to follow the target.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraControl : MonoBehaviour
{
    // * The offset between the camera and the target
    private Vector3 _offset;

    // * The target the camera will follow
    [SerializeField] private Transform target;

    // * The time it takes for the camera to catch up to the target
    [SerializeField] private float smoothTime;

    // * The current velocity of the camera, used by SmoothDamp
    private Vector3 _currentVelocity = Vector3.zero;

    private void Awake()
    {
        // * Calculate the initial offset between the camera and the target
        _offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        // * Calculate the target position of the camera
        Vector3 targetPosition = target.position + _offset;

        // * Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);
    }
}

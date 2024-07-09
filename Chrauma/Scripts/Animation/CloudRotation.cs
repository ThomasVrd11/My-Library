/*
 * ======================================================================================
 *                                 CloudRotation Script
 * ======================================================================================
 * This simple script handles the rotation of cloud objects in Unity. It continuously rotates
 * the object around the Y-axis to simulate the movement of clouds.
 *
 * Key Features:
 * - Rotates the cloud object around the Y-axis.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRotation : MonoBehaviour
{
    // * Update is called once per frame
    void Update()
    {
        // * Rotate the object around the Y-axis
        this.transform.Rotate(new Vector3(0, 1, 0) * 4 * Time.deltaTime);
    }
}

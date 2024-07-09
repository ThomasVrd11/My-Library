/*
 * ======================================================================================
 *                               PersistentManager Script
 * ======================================================================================
 * This script manages a persistent singleton instance across different scenes in Unity.
 * Ensures only one instance of the manager exists at any time.
 *
 * Key Features:
 * - Implements singleton pattern to maintain a single instance.
 * - Ensures the manager persists across different scenes.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager instance;

    private void Awake()
    {
        // * Ensure only one instance of the PersistentManager exists
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // * Make the instance persistent across scenes
        }
    }
}

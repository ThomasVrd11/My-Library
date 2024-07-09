/*
 * ======================================================================================
 *                          DontDestroyOnLoad Script
 * ======================================================================================
 * This script ensures that the GameObject it is attached to is not destroyed when loading
 * a new scene. This is useful for objects that need to persist across multiple scenes,
 * such as managers or audio sources.
 *
 * Key Features:
 * - Prevents the destruction of the GameObject when a new scene is loaded.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    // * Start is called before the first frame update
    private void Awake() {
        DontDestroyOnLoad(this);
    }
}

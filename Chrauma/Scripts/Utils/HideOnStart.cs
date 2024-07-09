/*
 * ======================================================================================
 *                             HideOnStart Script
 * ======================================================================================
 * This script hides the GameObject it is attached to when the game starts.
 *
 * Key Features:
 * - Deactivates the GameObject at the start of the game.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnStart : MonoBehaviour
{
    // * Start is called before the first frame update
    void Start()
    {
        // * Deactivate the GameObject at the start
        gameObject.SetActive(false);
    }

    // * Update is called once per frame
    void Update()
    {
        // * No update logic needed for this script
    }
}

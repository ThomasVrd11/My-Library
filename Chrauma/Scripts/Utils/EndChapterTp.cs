/*
 * ======================================================================================
 *                           EndChapterTp Script
 * ======================================================================================
 * This script manages the teleportation at the end of a chapter. The portal becomes
 * active when a certain number of enemies are killed, and upon entering the portal, 
 * the scene switches to the next map.
 *
 * Key Features:
 * - Activates the portal after a specified number of enemies are killed.
 * - Transitions to the next scene when the player enters the portal.
 * - Provides a smooth scaling animation when the portal appears.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndChapterTp : MonoBehaviour
{
    // * Index of the next map to load
    [SerializeField] private int nextMapIndex;

    // * Reference to the Spawner script
    [SerializeField] private Spawner spawner;

    // * CapsuleCollider component of the portal
    private CapsuleCollider cc;

    // * Flag to track if the portal has appeared
    private bool hasAppeared = false;

    private void Awake()
    {
        // * Get the CapsuleCollider component
        cc = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        // * Check if the spawner exists and if the required number of enemies are killed
        if (spawner)
        {
            if (spawner.numberOfKilledEnnemies == 3 && !hasAppeared)
            {
                StartCoroutine(PortalAppear());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // * Switch to the next scene when the player enters the portal
        GameManager.instance.SwitchScene(nextMapIndex);
    }

    IEnumerator PortalAppear()
    {
        // * Animate the portal appearing by scaling it up smoothly
        Vector3 initialScale = gameObject.transform.localScale;
        float currentTime = 0f;
        while (currentTime < 2.5f)
        {
            float newScaleY = Mathf.Lerp(initialScale.y, 1, currentTime / 2.5f);
            transform.localScale = new Vector3(initialScale.x, newScaleY, initialScale.z);
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(initialScale.x, 1, initialScale.z);
        cc.enabled = true;
        hasAppeared = true;
    }
}

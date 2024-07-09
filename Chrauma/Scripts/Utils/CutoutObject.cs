/*
 * ======================================================================================
 *                            CutoutObject Script
 * ======================================================================================
 * This script manages the cutout effect for objects that obstruct the view between
 * the camera and a target object. It dynamically updates the cutout effect to create
 * a clear line of sight to the target.
 *
 * Key Features:
 * - Detects objects obstructing the view to the target.
 * - Applies a cutout effect to the obstructing objects.
 * - Resets the cutout effect when objects are no longer obstructing the view.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField] private Transform targetObject; // * The target object to keep in view
    [SerializeField] private LayerMask wallMask;     // * Layer mask to identify obstructing objects
    private Camera mainCamera;                       // * Main camera reference
    private HashSet<Renderer> lastAffectedRenderers = new HashSet<Renderer>(); // * Track renderers affected by cutout
    private Vector3 lastPosition;                    // * Last position of the target object

    private void Awake()
    {
        // * Initialize the main camera and store the initial position of the target object
        mainCamera = GetComponent<Camera>();
        lastPosition = targetObject.position;
    }

    void Update()
    {
        // * Update the cutout effect if the target object has moved significantly
        if (ShouldUpdateCutout())
        {
            UpdateCutout();
            lastPosition = targetObject.position;
        }
    }

    private bool ShouldUpdateCutout()
    {
        // * Check if the target object has moved more than 0.5 units
        float distanceMoved = Vector3.Distance(lastPosition, targetObject.position);
        return distanceMoved > 0.5f;
    }

    private void UpdateCutout()
    {
        // * Calculate the cutout position and direction
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);
        Debug.DrawRay(transform.position, offset, Color.red);

        HashSet<Renderer> currentlyAffectedRenderers = new HashSet<Renderer>();

        // * Apply cutout effect to all hit objects
        foreach (var hit in hitObjects)
        {
            ProcessAllRenderers(hit.transform, (renderer) =>
            {
                currentlyAffectedRenderers.Add(renderer);
                ApplyCutout(renderer, cutoutPos, 0.15f, 0.05f);
            });
        }

        // * Reset cutout effect on previously affected renderers that are no longer obstructing
        ResetOldCutouts(currentlyAffectedRenderers);
        lastAffectedRenderers = currentlyAffectedRenderers;
    }

    private void ApplyCutout(Renderer renderer, Vector2 cutoutPos, float size, float falloff)
    {
        // * Apply the cutout effect to a renderer using MaterialPropertyBlock
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propBlock);
        propBlock.SetVector("_CutoutPos", new Vector4(cutoutPos.x, cutoutPos.y, 0, 0));
        propBlock.SetFloat("_CutoutSize", size);
        propBlock.SetFloat("_FalloffSize", falloff);
        renderer.SetPropertyBlock(propBlock);
    }

    private void ResetOldCutouts(HashSet<Renderer> currentlyAffectedRenderers)
    {
        // * Reset the cutout effect on renderers that are no longer obstructing the view
        foreach (var renderer in lastAffectedRenderers)
        {
            if (!currentlyAffectedRenderers.Contains(renderer))
            {
                MaterialPropertyBlock propBlockDone = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(propBlockDone);
                propBlockDone.SetFloat("_CutoutSize", 0);
                renderer.SetPropertyBlock(propBlockDone);
            }
        }
    }

    private void ProcessAllRenderers(Transform root, System.Action<Renderer> process)
    {
        // * Recursively process all renderers in the hierarchy
        Renderer renderer = root.GetComponent<Renderer>();
        if (renderer != null)
        {
            process(renderer);
        }
        foreach (Transform child in root)
        {
            ProcessAllRenderers(child, process);
        }
    }
}

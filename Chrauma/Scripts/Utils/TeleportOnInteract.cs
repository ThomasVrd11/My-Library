/*
 * ======================================================================================
 *                        TeleportOnInteract Script
 * ======================================================================================
 * This script handles player teleportation when they interact with a portal.
 * The player is teleported to the receiving portal's location and orientation
 * when they press a specified key while inside the trigger area.
 *
 * Key Features:
 * - Detects when the player is in the trigger area.
 * - Teleports the player to the receiving portal on key press.
 * - Handles enabling and disabling of the character controller during teleportation.
 * ======================================================================================
 */

using UnityEngine;
using System.Collections;

public class TeleportOnInteract : MonoBehaviour
{
    // * Variables for the teleportation
    [SerializeField] Transform receivingPortal; // * The target portal to teleport to
    private bool isTeleporting = false; // * Flag to prevent multiple teleportations
    private bool playerIsInTrigger = false; // * Flag to check if the player is in the trigger area
    private GameObject player; // * Reference to the player GameObject
    private GameObject playerTrail; // * Reference to the player's trail effect
    private CharacterController characterController; // * Reference to the player's character controller

    void Start()
    {
        // * Initialize player references
        player = GameObject.FindWithTag("Player");
        playerTrail = player.transform.Find("ghost/TrailReap").gameObject;
        characterController = player.GetComponent<CharacterController>();
    }

    // * I setup input to G because Geleportation
    void Update()
    {
        // * Check for player input to initiate teleportation
        if (playerIsInTrigger && Input.GetKeyDown(KeyCode.G) && !isTeleporting)
        {
            StartCoroutine(TeleportPlayer());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // * Detect when the player enters the trigger area
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger");
            playerIsInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // * Detect when the player exits the trigger area
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger");
            playerIsInTrigger = false;
        }
    }

    // * TP logic
    private IEnumerator TeleportPlayer()
    {
        // * Disable the player's trail and set teleporting flag
        playerTrail.SetActive(false);
        isTeleporting = true;

        // * Calculate the new player position and rotation relative to the receiving portal
        Quaternion portalRotationDifference = receivingPortal.rotation * Quaternion.Inverse(transform.rotation);
        portalRotationDifference *= Quaternion.Euler(0f, 180f, 0f);
        Vector3 positionOffset = player.transform.position - transform.position;
        positionOffset = portalRotationDifference * positionOffset;
        Vector3 newPosition = receivingPortal.position + positionOffset;

        // * Disable the character controller, teleport the player, and re-enable the controller
        characterController.enabled = false;
        player.transform.SetPositionAndRotation(newPosition, player.transform.rotation * portalRotationDifference);
        playerIsInTrigger = false;
        characterController.enabled = true;
        playerTrail.SetActive(true);
        isTeleporting = false;

        yield return null;
    }
}

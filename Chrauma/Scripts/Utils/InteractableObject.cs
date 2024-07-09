/*
 * ======================================================================================
 *                         InteractableObject Script
 * ======================================================================================
 * This script manages the interaction between the player and an interactable object.
 * It displays a message when the player is nearby and handles the interaction logic.
 *
 * Key Features:
 * - Displays an interaction message when the player is near.
 * - Handles player input to trigger interactions.
 * - Shows a follow-up message after interaction.
 * ======================================================================================
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableObject : MonoBehaviour
{
    public string interactMessage = "Press G to interact"; // * Message displayed when the player can interact
    public GameObject messagePrefab; // * Prefab for the message display
    private TMP_Text messageText; // * Text component for the message
    private bool isPlayerNearby = false; // * Flag to check if the player is nearby
    private bool messageDisplayed = false; // * Flag to check if the message is displayed
    [SerializeField] string messageAfter; // * Message displayed after interaction

    void Start()
    {
        // * Initialize the message text and hide the message prefab
        if (messagePrefab != null)
        {
            messageText = messagePrefab.transform.Find("MsgCanvas/Text (TMP)").GetComponent<TMP_Text>();
            messageText.text = "";
            messagePrefab.SetActive(false);
        }
    }

    void Update()
    {
        // * Check for player interaction input
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(DisplayMessage(messageAfter, 3f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // * Display the interaction message when the player is nearby
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (!messageDisplayed)
            {
                StartCoroutine(DisplayMessage(interactMessage, 30f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // * Hide the interaction message when the player leaves
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (messageDisplayed)
            {
                messagePrefab.SetActive(false);
                messageDisplayed = false;
            }
        }
    }

    private IEnumerator DisplayMessage(string message, float delay)
    {
        // * Display a message for a specified duration
        if (messageText != null)
        {
            messagePrefab.SetActive(true);
            messageText.text = message;
            messageDisplayed = true;
            yield return new WaitForSeconds(delay);
            messagePrefab.SetActive(false);
            messageDisplayed = false;
        }
    }
}

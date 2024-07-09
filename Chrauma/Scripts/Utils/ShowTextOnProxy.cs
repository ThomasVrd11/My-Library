/*
 * ======================================================================================
 *                          ShowTextOnProxy Script
 * ======================================================================================
 * This script displays a text message when the player enters a trigger area.
 * The text is displayed for a specified duration and then hides the GameObject.
 *
 * Key Features:
 * - Displays a message when the player enters the trigger area.
 * - Hides the message after a specified duration.
 * - Deactivates the GameObject after displaying the message.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowTextOnProxy : MonoBehaviour
{
    public GameObject player; // * The player GameObject
    public TMP_Text proximityText; // * The TextMeshPro text component
    public string message = "Displayed text"; // * The message to display
    private bool isTextDisplayed = false; // * Flag to check if the text is already displayed

    void Start()
    {
        // * Disable the text component at the start
        proximityText.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // * Display the message when the player enters the trigger area
        if (other.gameObject == player && !isTextDisplayed)
        {
            StartCoroutine(DisplayText());
        }
    }

    private IEnumerator DisplayText()
    {
        // * Display the message for 3 seconds
        isTextDisplayed = true;
        proximityText.text = message;
        proximityText.enabled = true;

        yield return new WaitForSeconds(3f);
        proximityText.enabled = false;
        isTextDisplayed = false;
        
        // * Deactivate the GameObject after displaying the message
        transform.gameObject.SetActive(false);
    }
}

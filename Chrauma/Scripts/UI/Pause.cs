/*
 * ======================================================================================
 *                              Pause Script
 * ======================================================================================
 * This script manages the pause functionality in the game. It toggles the game state
 * between paused and unpaused, and handles confirmation dialogs for save, load, and quit actions.
 *
 * Key Features:
 * - Toggles the game pause state.
 * - Displays pause and confirmation panels.
 * - Handles save, load, and quit confirmations.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    UILinker uiLinker;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject confirmPanel;
    [SerializeField] TMP_Text confirmText;
    [SerializeField] GameObject saveConfirmButton;
    [SerializeField] GameObject loadConfirmButton;
    [SerializeField] GameObject quitConfirmButton;

    bool pauseState = false;

    private void Awake()
    {
        // * Get the UILinker component
        uiLinker = GetComponent<UILinker>();
    }

    public void PressPause()
    {
        // * Toggle the pause state
        pauseState = !pauseState;
        HandlePause();
    }
    
    private void HandlePause()
    {
        // * Handle the game pause state and UI
        if (pauseState)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        } 
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        }
    }

    public void ConfirmChoice(int choice)
    {
        // * Display confirmation dialog based on the choice
        confirmPanel.SetActive(true);
        switch (choice)
        {
            case 0:
                confirmText.text = "This will override your previous save.\nAre you sure?";
                DeactivateUnnecessaryButtons();
                saveConfirmButton.SetActive(true);
                break;
            case 1:
                confirmText.text = "You will lose all unsaved data.\nAre you sure?";
                DeactivateUnnecessaryButtons();
                loadConfirmButton.SetActive(true);
                break;
            case 2:
                confirmText.text = "You will lose all unsaved data.\nAre you sure?";
                DeactivateUnnecessaryButtons();
                quitConfirmButton.SetActive(true);
                break;
        }
    }

    private void DeactivateUnnecessaryButtons()
    {
        // * Deactivate all confirmation buttons
        saveConfirmButton.SetActive(false);
        loadConfirmButton.SetActive(false);
        quitConfirmButton.SetActive(false);
    }
}

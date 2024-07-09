/*
 * ======================================================================================
 *                                 TutorialTriggers Script
 * ======================================================================================
 * This script handles the various tutorial triggers in the game. It provides functionality
 * for different tutorial stages like movement, dash, and attack tutorials. It also manages
 * the game state, pausing the game when a tutorial starts and resuming it when the tutorial
 * ends. Additionally, it handles player repositioning to prevent falling off the map.
 *
 * Key Features:
 * - Starts specific tutorials based on trigger collision.
 * - Pauses and resumes the game.
 * - Repositions player to prevent falling.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTriggers : MonoBehaviour
{
    [SerializeField] GameObject fallPreventPos;
    [SerializeField] GameObject pauseUI;
    [SerializeField] GameObject movementTuto;
    [SerializeField] GameObject dashTuto;
    [SerializeField] GameObject attackTuto;
    [SerializeField] GameObject fightingUI;
    [SerializeField] Spawner spawner;
    [SerializeField] GameObject tpEndOfTuto;
    [SerializeField] Slider healthSlider;

    private CharacterControls characterControls;

    private void OnTriggerEnter(Collider other)
    {
        // * Check if the player entered the trigger
        if (other.gameObject.name == "Player")
        {
            characterControls = other.GetComponent<CharacterControls>();
            if (gameObject.name == "DashTutoCube")
            {
                StartDashTuto();
            }
            else if (gameObject.name == "AttackTutoCube")
            {
                StartAttackTuto();
            }
            else if (gameObject.name == "BridgeHole")
            {
                // * Prevent player from falling by repositioning
                CharacterController playerCC = other.gameObject.GetComponent<CharacterController>();
                playerCC.enabled = false;
                other.transform.SetPositionAndRotation(fallPreventPos.transform.position, fallPreventPos.transform.rotation);
                playerCC.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // * Deactivate the tutorial object and resume game
        gameObject.SetActive(false);
        if (characterControls) characterControls.gamePaused = false;
    }

    public void Resume()
    {
        // * Resume game from pause
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
    }

    public void Pause()
    {
        // * Pause the game and show pause UI
        if (characterControls) characterControls.gamePaused = true;
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
    }

    public void StartMovTuto()
    {
        // * Start movement tutorial
        movementTuto.SetActive(true);
        Pause();
    }

    public void StartDashTuto()
    {
        // * Start dash tutorial
        dashTuto.SetActive(true);
        Pause();
    }

    private void StartAttackTuto()
    {
        // * Start attack tutorial
        attackTuto.SetActive(true);
        tpEndOfTuto.SetActive(true);
        Pause();
        fightingUI.SetActive(true);
        PlayerStats.instance.SetHealthBar(healthSlider);
        spawner.isSpawningActive = true;
    }
}

/*
 * ======================================================================================
 *                             UILinker Script
 * ======================================================================================
 * This script provides a link between the UI and various game management functionalities.
 * It allows the UI to trigger game saving, loading, exiting, restarting, and volume control.
 *
 * Key Features:
 * - Links UI actions to data persistence, game management, and audio management functions.
 * - Simplifies the connection between UI elements and game logic.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILinker : MonoBehaviour
{
    public void Load()
    {
        // * Load the game data
        DataPersistenceManager.instance.LoadGame();
    }

    public void Save()
    {
        // * Save the game data
        DataPersistenceManager.instance.SaveGame();
    }

    public void Exit()
    {
        // * Exit the game
        GameManager.instance.ExitGame();
    }

    public void Restart()
    {
        // * Hide the death screen and restart the game
        PlayerStats.instance.HideDeath();
        GameManager.instance.RestartGame();
    }

    public void SetVolMaster(float volume)
    {
        // * Set the master volume
        AudioManager.instance.SetVolumeMaster(volume);
    }

    public void SetVolMusic(float volume)
    {
        // * Set the music volume
        AudioManager.instance.SetVolumeMusic(volume);
    }

    public void SetVolSFX(float volume)
    {
        // * Set the SFX volume
        AudioManager.instance.SetVolumeSFX(volume);
    }

    public void SaveVolSettings()
    {
        // * Save the volume settings
        AudioManager.instance.SaveVolumeSettings();
    }
}

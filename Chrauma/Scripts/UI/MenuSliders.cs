/*
 * ======================================================================================
 *                              MenuSliders Script
 * ======================================================================================
 * This script manages the volume sliders in the game menu, loading their values from
 * PlayerPrefs at the start of the game. It initializes the sliders with the saved volume
 * levels for master, music, and SFX.
 *
 * Key Features:
 * - Loads saved volume levels from PlayerPrefs.
 * - Initializes the volume sliders with the saved values.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSliders : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // * Load and set the master volume slider value from PlayerPrefs
        if (PlayerPrefs.HasKey("volumeMasterPref"))
        {
            float savedMasterVolume = PlayerPrefs.GetFloat("volumeMasterPref");
            masterSlider.value = savedMasterVolume;
        }

        // * Load and set the music volume slider value from PlayerPrefs
        if (PlayerPrefs.HasKey("volumeMusicPref"))
        {
            float savedMusicVolume = PlayerPrefs.GetFloat("volumeMusicPref");
            musicSlider.value = savedMusicVolume;
        }

        // * Load and set the SFX volume slider value from PlayerPrefs
        if (PlayerPrefs.HasKey("volumeSFXPref"))
        {
            float savedSFXVolume = PlayerPrefs.GetFloat("volumeSFXPref");
            sfxSlider.value = savedSFXVolume;
        }
    }
}

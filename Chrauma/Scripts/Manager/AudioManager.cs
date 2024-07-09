/*
 * ======================================================================================
 *                               AudioManager Script
 * ======================================================================================
 * This script manages the audio settings and background music for different scenes in Unity.
 * It implements a singleton pattern to ensure only one instance of the AudioManager exists.
 * It handles setting and saving volume settings for master, music, and SFX channels.
 *
 * Key Features:
 * - Manages volume settings and saves them using PlayerPrefs.
 * - Plays background music based on the current scene.
 * - Ensures a single instance of AudioManager across scenes.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mixer;
    private AudioSource bgmPlayer;
    public List<AudioClip> bmgTracks;

    private void Awake()
    {
        // * Ensure only one instance of the AudioManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        bgmPlayer = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // * Load volume settings from PlayerPrefs
        if (PlayerPrefs.HasKey("volumeMasterPref"))
            SetVolumeMaster(PlayerPrefs.GetFloat("volumeMasterPref"));
        if (PlayerPrefs.HasKey("volumeMusicPref"))
            SetVolumeMusic(PlayerPrefs.GetFloat("volumeMusicPref"));
        if (PlayerPrefs.HasKey("volumeSFXPref"))
            SetVolumeSFX(PlayerPrefs.GetFloat("volumeSFXPref"));
    }

    public void SetVolumeMaster(float volumeMaster)
    {
        // * Set the master volume
        mixer.SetFloat("VolumeMaster", Mathf.Log10(volumeMaster) * 20);
    }

    public void SetVolumeMusic(float volumeMusic)
    {
        // * Set the music volume
        mixer.SetFloat("VolumeMusic", Mathf.Log10(volumeMusic) * 20);
    }

    public void SetVolumeSFX(float volumeSFX)
    {
        // * Set the SFX volume
        mixer.SetFloat("VolumeSFX", Mathf.Log10(volumeSFX) * 20);
    }

    public void SaveVolumeSettings()
    {
        // * Save the current volume settings to PlayerPrefs
        float volumeMaster;
        if (mixer.GetFloat("VolumeMaster", out volumeMaster))
        {
            PlayerPrefs.SetFloat("volumeMasterPref", Mathf.Pow(10, volumeMaster / 20));
        }

        float volumeMusic;
        if (mixer.GetFloat("VolumeMusic", out volumeMusic))
        {
            PlayerPrefs.SetFloat("volumeMusicPref", Mathf.Pow(10, volumeMusic / 20));
        }

        float volumeSFX;
        if (mixer.GetFloat("VolumeSFX", out volumeSFX))
        {
            PlayerPrefs.SetFloat("volumeSFXPref", Mathf.Pow(10, volumeSFX / 20));
        }

        PlayerPrefs.Save();
    }

    private void OnEnable()
    {
        // * Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // * Unsubscribe from the sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // * Play background music based on the current scene
        int sceneIndex = scene.buildIndex;

        if (sceneIndex < bmgTracks.Count && bmgTracks[sceneIndex] != null)
        {
            bgmPlayer.clip = bmgTracks[sceneIndex];
            bgmPlayer.Play();
        }
    }
}

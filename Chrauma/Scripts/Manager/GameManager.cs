/*
 * ======================================================================================
 *                              GameManager Script
 * ======================================================================================
 * This script manages the overall game state, including scene transitions, game exit,
 * and game restart. It implements a singleton pattern to ensure only one instance exists.
 *
 * Key Features:
 * - Manages scene transitions.
 * - Handles game exit and restart.
 * - Implements singleton pattern for centralized game management.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake() {
        // * Ensure only one instance of the GameManager exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // * This section is for handling cursor visibility and loading game state
        // #if !UNITY_EDITOR
        // Cursor.visible = false;
        // #endif

        // * If save exists, enable the continue button and load stats from save
        // * If new game, set default stats
    }

    // * Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchScene(int sceneId)
    {
        // * Load the scene asynchronously by sceneId
        SceneManager.LoadSceneAsync(sceneId);
    }

    public void ExitGame()
    {
        // * Exit the game or stop playing in the Unity Editor
        #if UNITY_EDITOR
            // If running in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // If running in a standalone build
            Application.Quit();
        #endif
    }

    public void RestartGame()
    {
        // * Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

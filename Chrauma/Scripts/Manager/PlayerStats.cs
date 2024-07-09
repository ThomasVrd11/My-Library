/*
 * ======================================================================================
 *                             PlayerStats Script
 * ======================================================================================
 * This script manages the player's stats, such as health and entropy, and updates the UI
 * accordingly. It also handles saving and loading player data, as well as player death.
 *
 * Key Features:
 * - Manages player health and entropy.
 * - Updates UI sliders to reflect current stats.
 * - Handles player damage, healing, and death.
 * - Implements data persistence for saving and loading player stats.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IDataPersistence
{
    public static PlayerStats instance;
    private int max_health;
    private int max_entropy;
    [SerializeField] int current_health;
    [SerializeField] int current_entropy;
    [SerializeField] private int buffer_health;
    private int buffer_entropy;
    private GameObject UI;
    private Slider slider_health;
    private Slider slider_entropy;
    public bool debugMode = false;
    public bool debugDeath = false;
    private GameObject deathScreen;

    private void Awake()
    {
        // * Initialize max health and entropy
        max_health = 100;
        max_entropy = 100;
    }

    void Start()
    {
        instance = this;
        InitializeUI();
        // * Setup stats
        current_health = max_health;
        current_entropy = max_entropy;
        buffer_entropy = max_entropy;
        buffer_health = max_health;
        UpdateSliders();
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

    void Update()
    {
        // * Update health and entropy buffers if they have changed
        if (buffer_health != current_health)
        {
            current_health = buffer_health;
            UpdateSliders();
        }
        if (buffer_entropy != current_entropy)
        {
            UpdateSliders();
        }
        if (debugDeath) Death();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // * Initialize UI elements when a new scene is loaded
        InitializeUI();
    }

    private void InitializeUI()
    {
        UI = GameObject.Find("---- UI ----");
        if (UI != null)
        {
            Transform healthTransform = UI.transform.Find("HealthBar_");
            if (healthTransform != null)
            {
                slider_health = healthTransform.GetComponent<Slider>();
            }

            Transform entropyTransform = UI.transform.Find("EntropyBar_");
            if (entropyTransform != null)
            {
                slider_entropy = entropyTransform.GetComponent<Slider>();
            }
            UpdateSliders();
        }
    }

    // * Update UI sliders for health and entropy
    private void UpdateSliders()
    {
        if (slider_health != null)
        {
            slider_health.value = current_health;
            if (debugMode) Debug.Log("health has been updated to: " + slider_health.value);
        }
        if (slider_entropy != null)
            slider_entropy.value = current_entropy;
    }

    public void TakeDamage(int damage)
    {
        // * Reduce health by damage amount
        buffer_health -= damage;
        if (buffer_health < 0) Death();
    }

    public void Heal(int heal)
    {
        // * Increase health by heal amount
        buffer_health += heal;
    }

    public void LoadData(GameData data)
    {
        // * Load player health and entropy from saved data
        this.buffer_health = data.health;
        this.buffer_entropy = data.entropy;
    }

    public void SaveData(GameData data)
    {
        // * Save player health and entropy to game data
        if (data == null)
        {
            Debug.LogError("GameData is null in playerstats.SaveData");
            return;
        }
        data.health = this.current_health;
        data.entropy = this.current_entropy;
    }

    public void SetHealthBar(Slider slider)
    {
        // * Set the health bar slider
        slider_health = slider;
        if (debugMode) Debug.Log("health set to: " + slider_health);
        UpdateSliders();
    }

    public void Death()
    {
        // * Handle player death and display death screen
        deathScreen = GameObject.Find("DeathScreen");
        if (deathScreen)
        {
            deathScreen.transform.Find("Panel").gameObject.SetActive(true);
        }
    }

    public void HideDeath()
    {
        // * Hide the death screen
        if (deathScreen)
        {
            deathScreen.transform.Find("Panel").gameObject.SetActive(false);
            Debug.Log("Blergh!!");
        }
    }
}

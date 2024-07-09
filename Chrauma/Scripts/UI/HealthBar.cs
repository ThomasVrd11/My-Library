/*
 * ======================================================================================
 *                               HealthBar Script
 * ======================================================================================
 * This script manages the health bar UI element, updating its value based on the player's
 * current health. It initializes the health at the start and continuously updates the 
 * health bar's value in the Update method.
 *
 * Key Features:
 * - Initializes player's health to the maximum value.
 * - Continuously updates the health bar value to match the current health.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // * Slider UI element for displaying health
    public Slider healthSlider;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float health;

    void Start()
    {
        // * Initialize health to the maximum value at the start
        health = maxHealth;
    }

    void Update()
    {
        // * Update the health bar value if it doesn't match the current health
        if (healthSlider.value != health)
        {
            healthSlider.value = health;
        }
    }
}

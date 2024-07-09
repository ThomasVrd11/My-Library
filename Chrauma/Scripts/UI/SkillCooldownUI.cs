/*
 * ======================================================================================
 *                          SkillCooldownUI Script
 * ======================================================================================
 * This script manages the UI for skill cooldowns, displaying an overlay that fills
 * or empties based on the remaining cooldown time for a skill.
 *
 * Key Features:
 * - Initializes the cooldown overlay.
 * - Starts the cooldown and updates the overlay based on the cooldown time.
 * ======================================================================================
 */

using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    // * Reference to the skill icon image
    public Image skillIcon; 

    // * Reference to the cooldown overlay image
    public Image cooldownOverlay;

    private float cooldownTime;
    private float cooldownRemaining;

    void Start()
    {
        // * Initialize the cooldown overlay fill amount to 0
        cooldownOverlay.fillAmount = 0;
    }

    public void StartCooldown(float duration)
    {
        // * Set the cooldown time and initialize the remaining cooldown
        cooldownTime = duration;
        cooldownRemaining = duration;
        cooldownOverlay.fillAmount = 1;
    }

    void Update()
    {
        // * Update the cooldown overlay fill amount based on the remaining cooldown time
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
            cooldownOverlay.fillAmount = cooldownRemaining / cooldownTime;
        }
        else if (cooldownOverlay.fillAmount > 0)
        {
            cooldownOverlay.fillAmount = 0;
        }
    }
}

/*
 * ======================================================================================
 *                             MainMenuManager Script
 * ======================================================================================
 * This script manages the main menu interactions and visual effects, such as rotating
 * the player character plate, changing saturation, and handling game start events.
 *
 * Key Features:
 * - Changes saturation over time using post-processing effects.
 * - Rotates the player character plate until a new game is started.
 * - Manages the transition from the main menu to the game scene.
 * - Checks for saved game data and enables the continue button if available.
 * - Prompts the player if they want to override an existing save when starting a new game.
 * ======================================================================================
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // * Post processing volume
    [SerializeField] Volume volume;
    // * Duration for saturation change
    [SerializeField] float duration = 5.0f;
    // * Animator for the player character
    [SerializeField] Animator playerAnimator;
    // * Object containing the player
    [SerializeField] GameObject playerPlate;
    // * Main Camera
    [SerializeField] GameObject mainCamera;
    // * Cinematic camera
    [SerializeField] GameObject cinematicCamera;
    // * Canvas group for fading UI
    [SerializeField] CanvasGroup canvasGroup;
    // * Continue button
    [SerializeField] Button continueButton;
    // * Override save prompt
    [SerializeField] GameObject overrideSave;

    // * Color adjustments for post-process
    private ColorAdjustments colorAdjustments;
    // * Flag to check if new game is started
    private bool selectedStart = false;

    private void Start()
    {
        // *
        // * Try to get the colorAdjustments component from volume profile
        // * Check if save game exists to enable continue button
        // *
        if (volume.profile.TryGet(out colorAdjustments))
        {
            StartCoroutine(ChangeSaturation());
        }
        if (DataPersistenceManager.instance.CheckIfSave())
        {
            continueButton.interactable = true;
        }
    }

    void Update()
    {
        // * Make the player plate rotate until a new game is started
        if (!selectedStart)
        {
            Vector3 currentRotation = mainCamera.transform.eulerAngles;
            currentRotation.y += 4 * Time.deltaTime;
            mainCamera.transform.eulerAngles = currentRotation;
            playerPlate.transform.Rotate(new Vector3(0, 1, 0) * 4 * Time.deltaTime);
        }
    }

    IEnumerator ChangeSaturation()
    {
        // * Coroutine to change the saturation over time
        while (true)
        {
            yield return new WaitForSeconds(15);
            float currentTime = 0f;
            while (currentTime <= duration)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(-100, 0, currentTime / duration);
                currentTime += Time.deltaTime;
                yield return null;
            }
            while (currentTime >= 0)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(-100, 0, currentTime / duration);
                currentTime -= Time.deltaTime;
                yield return null;
            }
        }
    }

    public void startTheGame()
    {
        // * Start the animation and scene switch when starting a new game
        int isDed = Animator.StringToHash("isDed");
        playerAnimator.SetBool(isDed, true);
        mainCamera.SetActive(false);
        cinematicCamera.SetActive(true);
        selectedStart = true;
        StartCoroutine(FadeCanvasGroup());
        StartCoroutine(launchGame());
    }

    private IEnumerator FadeCanvasGroup()
    {
        // * Fade the canvas alpha to 0 over time
        float timeStartedLerping = Time.time;
        float timeSinceStarted = 0f;
        float percentageComplete = 0f;

        while (percentageComplete < 1)
        {
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / 1;
            float currentValue = Mathf.Lerp(1, 0, percentageComplete);

            canvasGroup.alpha = currentValue;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator launchGame()
    {
        // * Start the game after a delay
        yield return new WaitForSeconds(5);
        GameManager.instance.SwitchScene(1);
    }

    public void CheckOverride()
    {
        // * Checks if there is a save file, and prompt the override warning if it exists
        if (!DataPersistenceManager.instance.CheckIfSave())
        {
            DataPersistenceManager.instance.NewGame();
            GameObject.Find("MainMenuManager").GetComponent<MainMenuManager>().startTheGame();
        }
        else
        {
            overrideSave.SetActive(true);
            GameObject.Find("LeftMenu").SetActive(false);
        }
    }
}

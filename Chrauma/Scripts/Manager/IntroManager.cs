/*
 * ======================================================================================
 *                              IntroManager Script
 * ======================================================================================
 * This script manages the introduction sequence for the game. It handles the initial
 * camera setup, player controls, and tutorial triggers. It also manages the fading of
 * introductory text and animations for the introduction scene.
 *
 * Key Features:
 * - Manages the initial camera setup and transitions.
 * - Controls the player and tutorial triggers during the intro sequence.
 * - Fades in and out introductory text.
 * - Handles animations and visual effects for the intro scene.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera introCam;
    [SerializeField] CinemachineVirtualCamera mainCam;
    [SerializeField] Canvas Tutorials;
    [SerializeField] GameObject lookingAroundTarget;
    [SerializeField] Animator lilGhostAnimator;
    [SerializeField] GameObject[] characterDisplay;
    // 0-reaper body 1-scythe 2-lilghost body

    [SerializeField] GameObject player;
    [SerializeField] float lookAroundTime;
    [SerializeField] Renderer ghostRenderer;
    [SerializeField] CharacterControls playerControls;
    [SerializeField] TutorialTriggers tutorialTriggers;
    [SerializeField] GameObject introTxt;
    [SerializeField] TMP_Text first_text;
    [SerializeField] TMP_Text second_text;
    private Vector3 posLook1 = new Vector3(-129.06f, 23.90f, -16.44f);
    private Vector3 posLook2 = new Vector3(-123f, 19f, -15f);

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return null;
        // * Activate intro camera and deactivate main camera
        introCam.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(false);
        // * Set initial character display states
        characterDisplay[0].SetActive(false);
        characterDisplay[1].SetActive(false);
        characterDisplay[2].SetActive(true);
        // * Disable player controls
        playerControls.enabled = false;
        StartCoroutine(WhatHappened());
    }

    IEnumerator WhatHappened()
    {
        // *
        // * Activate the intro text panel
        // * Fade in the first text, wait, then fade out
        // * Repeat for the second text
        // * Start the intro scene
        // *
        introTxt.gameObject.SetActive(true);
        StartCoroutine(Fade(first_text, true));
        yield return new WaitForSeconds(3);
        StartCoroutine(Fade(first_text, false));
        yield return new WaitForSeconds(3);
        StartCoroutine(Fade(second_text, true));
        yield return new WaitForSeconds(3);
        StartCoroutine(Fade(second_text, false));
        yield return new WaitForSeconds(3);
        introTxt.gameObject.SetActive(false);
        StartCoroutine(IntroSceneStart());
        yield return null;
    }

    IEnumerator Fade(TMP_Text textToFade, bool inOut)
    {
        // *
        // * Check the bool inOut, if true opacity will go from 0 to 1, otherwise from 1 to 0
        // * While fadeTime is not reached, update the fade value over time
        // * Ensure the final alpha value is correctly set at the end
        // *
        float fadeTime = 1.5f;
        float elapsedTime = 0f;
        int startValue = 0;
        int endValue = 0;
        if (inOut)
        {
            endValue = 1;
        }
        else
        {
            startValue = 1;
        }
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float fadeValue = Mathf.Lerp(startValue, endValue, elapsedTime / fadeTime);
            textToFade.alpha = fadeValue;
            yield return null;
        }
        yield return null;
    }

    IEnumerator IntroSceneStart()
    {
        yield return new WaitForSeconds(1.5f);
        // * Switch from intro camera to main camera
        introCam.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(Apparition());
        // * Handle look around actions
        lookAround(lookingAroundTarget.transform.position);
        yield return new WaitForSeconds(lookAroundTime);
        lookAround(posLook2);
        yield return new WaitForSeconds(lookAroundTime);
        lookAround(posLook1);
        yield return new WaitForSeconds(0.5f);
        lilGhostAnimator.SetTrigger("Surprised");
        yield return new WaitForSeconds(0.7f);
        playerControls.enabled = true;
        tutorialTriggers.StartMovTuto();
        yield return null;
    }

    IEnumerator Apparition()
    {
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        float dissolveTime = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float dissolveValue = Mathf.Lerp(0, 1, elapsedTime / dissolveTime);

            ghostRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat("_Dissolve", dissolveValue);
            ghostRenderer.SetPropertyBlock(propBlock);

            yield return null;
        }
        ghostRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Dissolve", 1);
        ghostRenderer.SetPropertyBlock(propBlock);
    }

    private void lookAround(Vector3 pos)
    {
        // * Update the position of the looking around target and make the player look at it
        lookingAroundTarget.transform.position = pos;
        player.transform.LookAt(lookingAroundTarget.transform.position);
    }
}

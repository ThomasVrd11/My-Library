/*
 * ======================================================================================
 *                                 GetReaper Script
 * ======================================================================================
 * This script handles the transformation sequence of the player character into a reaper.
 * It involves camera transitions, animation triggers, visual effects, and modifying
 * post-processing effects. This script is reusable for any character transformation
 * sequence requiring a cinematic effect in Unity.
 *
 * Key Features:
 * - Disables player controls and triggers the transformation sequence.
 * - Switches between multiple virtual cameras.
 * - Triggers animations and visual effects.
 * - Adjusts color saturation during the transformation.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class GetReaper : MonoBehaviour
{
    [SerializeField] Transform tpDestination;
    [SerializeField] GameObject player;
    [SerializeField] GameObject effects;
    [SerializeField] List<GameObject> playerModels;
    [SerializeField] CinemachineVirtualCamera VCGetReaper;
    [SerializeField] CinemachineVirtualCamera VCGetReaper2;
    [SerializeField] CinemachineVirtualCamera VCFollowPlayer;
    [SerializeField] Animator ghostAnimator;
    [SerializeField] Volume volume;
    ColorAdjustments colorAdjustments;
    [SerializeField] MeshRenderer weaponMeshRenderer;

    private void OnTriggerEnter(Collider other)
    {
        // * Start the transformation sequence if the player enters the trigger
        if (other.gameObject.name == "Player")
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.GetComponent<CharacterControls>().enabled = false;
            effects.SetActive(true);
            StartCoroutine(TurnIntoReaper());
        }
    }

    IEnumerator TurnIntoReaper()
    {
        // * Player looks at the camera and the first camera is activated
        player.transform.LookAt(VCGetReaper.gameObject.transform.position);
        VCGetReaper.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);

        // * Trigger the ghost animator's "Surprised" animation
        ghostAnimator.SetTrigger("Surprised");
        yield return new WaitForSeconds(3);
        ghostAnimator.SetTrigger("Surprised");

        // * Disable weapon mesh renderer and start bringing back color adjustments
        weaponMeshRenderer.enabled = false;
        if (volume.profile.TryGet(out colorAdjustments))
        {
            StartCoroutine(BringColorBack(colorAdjustments));
        }

        // * Activate player models in sequence
        playerModels[0].SetActive(true);
        playerModels[2].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        playerModels[1].SetActive(false);

        // * Trigger player skill animation
        yield return new WaitForSeconds(0.5f);
        player.GetComponent<Animator>().SetTrigger("Skill1Stage");
        yield return new WaitForSeconds(4);

        // * Switch to the second camera
        VCGetReaper2.gameObject.SetActive(true);
        yield return new WaitForSeconds(8);

        // * Teleport player to the destination
        player.transform.position = tpDestination.position;
        yield return new WaitForSeconds(3);

        // * Adjust camera clipping plane and deactivate transformation cameras
        VCFollowPlayer.m_Lens.NearClipPlane = -20;
        VCGetReaper2.gameObject.SetActive(false);
        VCGetReaper.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);

        // * Re-enable player controls and destroy the trigger object
        player.GetComponent<CharacterControls>().enabled = true;
        player.GetComponent<CharacterController>().enabled = true;
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator BringColorBack(ColorAdjustments colorAdjustments)
    {
        // * Gradually adjust the saturation to bring back color
        float currentTime = 0f;
        while (currentTime <= 2.5f)
        {
            colorAdjustments.saturation.value = Mathf.Lerp(-30, 30, currentTime / 2.5f);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}

/*
 * ======================================================================================
 *                                 DoorCinematic Script
 * ======================================================================================
 * This script handles the cinematic sequence when the player approaches a specific door.
 * It switches between different camera perspectives, stops the player momentarily,
 * plays audio clips, and animates the door opening. This script can be reused for any
 * similar door interaction requiring a cinematic effect in Unity.
 *
 * Key Features:
 * - Switches between orthographic and perspective cameras.
 * - Plays audio clips for door and breath sounds.
 * - Temporarily stops the player's movement.
 * - Animates door opening.
 * ======================================================================================
 */

using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.AI;

public class DoorCinematic : MonoBehaviour
{
    public CinemachineVirtualCamera perspectiveCamera;
    public CinemachineVirtualCamera orthographicCamera;
    public CinemachineVirtualCamera cameraDoor2;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private NavMeshAgent navAgentPlayer;
    [SerializeField] private GameObject waypoint1;
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;
    private bool hasBeenTriggered = false;
    public float time = 5.0f;

    private Camera mainCamera;
    private bool isOrthographic = true;
    private AudioSource audioSource;
    public AudioClip breathAudio;
    public AudioClip doorAudio;

    void Start()
    {
        // * Initialize main camera and audio source
        mainCamera = Camera.main;
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // * Trigger the cinematic if the player enters the collider
        if (other.name == "Player" && isOrthographic && !hasBeenTriggered)
        {
            SwitchCam(isOrthographic);
            StartCoroutine(StopPlayerForASec(time));
            hasBeenTriggered = true;
        }
    }

    private void SwitchCam(bool isOrthographic)
    {
        // * Switch between orthographic and perspective cameras
        if (isOrthographic)
        {
            perspectiveCamera.gameObject.SetActive(true);
            orthographicCamera.gameObject.SetActive(false);
        }
        else
        {
            cameraDoor2.gameObject.SetActive(false);
            orthographicCamera.gameObject.SetActive(true);
        }
    }

    IEnumerator StopPlayerForASec(float timeStopped)
    {
        float elapsedTime = 0;

        // * Store initial and target rotations for doors
        Quaternion startRotLeft = doorLeft.transform.rotation;
        Quaternion startRotRight = doorRight.transform.rotation;
        Quaternion endRotLeft = Quaternion.Euler(0, -555f, 0);
        Quaternion endRotRight = Quaternion.Euler(0, 5f, 0);

        // * Disable character controller and play audio
        characterController.enabled = false;
        Debug.Log("cc: " + characterController.enabled);
        audioSource.PlayOneShot(doorAudio);
        audioSource.PlayOneShot(breathAudio);

        // * Animate door opening
        while (elapsedTime < 3f)
        {
            float t = elapsedTime / 3f;
            doorLeft.transform.rotation = Quaternion.Slerp(startRotLeft, endRotLeft, t);
            doorRight.transform.rotation = Quaternion.Slerp(startRotRight, endRotRight, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // * Move player to waypoint and switch cameras
        navAgentPlayer.enabled = true;
        navAgentPlayer.SetDestination(waypoint1.transform.position);
        StartCoroutine(AnimateDoorCamera());
        yield return new WaitForSeconds(timeStopped);

        // * Re-enable character controller and reset camera
        characterController.enabled = true;
        Debug.Log("cc: " + characterController.enabled);
        navAgentPlayer.enabled = false;
        orthographicCamera.m_Lens.NearClipPlane = -7.9f;
        SwitchCam(false);
    }

    IEnumerator AnimateDoorCamera()
    {
        // * Delay and then activate door camera
        yield return new WaitForSeconds(2);
        perspectiveCamera.gameObject.SetActive(false);
        cameraDoor2.gameObject.SetActive(true);
    }
}

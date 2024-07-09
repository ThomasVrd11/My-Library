/*
 * ======================================================================================
 *                                 CharacterControls Script
 * ======================================================================================
 * This script manages the player's character controls, including movement, dashing,
 * berserk mode, and skill activation. It integrates with Unity's Input System and handles
 * animation states, skill cooldowns, and game pausing functionality.
 *
 * Key Features:
 * - Handles player movement and rotation based on input.
 * - Manages dashing, berserk mode, and skills with cooldowns.
 * - Integrates with the Animator component to trigger animations.
 * - Supports game pause functionality.
 * - Implements data persistence for saving and loading player position.
 * ======================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterControls : MonoBehaviour, IDataPersistence
{
    // * ########## Variables ########## * //
    PlayerInputs playerInput;
    CharacterController characterController;
    Animator animator;
    [SerializeField] WeaponDamage scytheDamage;

    // * ########## Hashes ########## * //
    int berserkTriggerHash;
    int skill1TriggerHash;
    int skill2TriggerHash;
    int skill3TriggerHash;

    // * ########## Input Values ########## * //
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 appliedMovement;
    bool isMovementPressed;
    bool skill1Pressed = false;

    // * ########## Constants movement ########## * //
    float normalSpeed = 4.5f;
    public float dashSpeed = 100f;
    public float dashDuration = 0.05f;
    float dashCooldown = 1.5f;
    float dashTimeLeft = 0;
    float dashCooldownLeft = 0;
    float speed = 4.5f;

    // * ########## Berserk ########## * //
    public float berserkDuration = 10.0f;
    private bool isBerserk = false;
    private float berserkEndTime;

    // * ########## Skill Variables ########## * //
    private float skill1TimeLeft = 0;
    private float skill2TimeLeft = 0;
    private float skill3TimeLeft = 0;
    private int skill1Stage = 0;
    private int skill2Stage = 0;
    private int skill3Stage = 0;
    private float skillStageDuration = 1.5f;

    // * ########## UI ELEMENTS ########## * //
    public SkillCooldownUI berserkCooldownUI;
    private Pause pauseMenu;
    public bool gamePaused;

    // * ########## Camera ########## * //
    [SerializeField] Camera mainCamera;
    
    // * ########## Sound ########## * //
    private AudioSource audioSource;
    public AudioClip dashSound;
    public AudioClip[] attackSound;
    
    public bool debugMode;

    // * ########## Functions ########## * //
    void Awake()
    {
        // * Initialize input, character controller, animator, and audio source
        playerInput = new PlayerInputs();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // * Initialize animation hashes
        berserkTriggerHash = Animator.StringToHash("Berserk");
        skill1TriggerHash = Animator.StringToHash("Skill1Stage");
        skill2TriggerHash = Animator.StringToHash("Skill2Stage");
        skill3TriggerHash = Animator.StringToHash("Skill3Stage");

        // * Set up input callbacks
        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Dash.performed += onDash;
        playerInput.CharacterControls.Berserk.performed += onBerserk;
        playerInput.CharacterControls.Skill1.performed += onSkill1;
        playerInput.CharacterControls.Skill2.performed += onSkill2;
        playerInput.CharacterControls.Skill3.performed += onSkill3;
        playerInput.CharacterControls.Pause.performed += onPause;
    }

    private void Start()
    {
        // * Find and assign the pause menu
        pauseMenu = GameObject.Find("PauseMenu").GetComponent<Pause>();
    }

    void Update()
    {
        // * Update dash cooldown and timing
        if (dashCooldownLeft > 0)
        {
            dashCooldownLeft -= Time.deltaTime;
        }

        if (dashTimeLeft > 0)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                speed = isBerserk ? normalSpeed * 1.5f : normalSpeed;
            }
        }

        // * Check if berserk mode should end
        if (isBerserk && Time.time > berserkEndTime)
        {
            DeactivateBerserk();
        }

        UpdateSkillTimers();
        HandleMovement();
        HandleLookDirection();
        UpdateAnimatorParameters();
        appliedMovement.x = currentMovement.x;
        appliedMovement.z = currentMovement.z;
        if (characterController.enabled)
            characterController.Move(appliedMovement * Time.deltaTime);
    }

    void UpdateSkillTimers()
    {
        // * Update timers for skills and reset stages if necessary
        if (skill1TimeLeft > 0)
        {
            skill1TimeLeft -= Time.deltaTime;
            if (skill1TimeLeft <= 0)
            {
                skill1Stage = 0;
                animator.ResetTrigger(skill1TriggerHash);
                if (debugMode) Debug.Log("Skill1 Timer Expired: Resetting to Stage 0");
            }
        }
        if (skill2TimeLeft > 0)
        {
            skill2TimeLeft -= Time.deltaTime;
            if (skill2TimeLeft <= 0)
            {
                skill2Stage = 0;
                animator.ResetTrigger(skill2TriggerHash);
                if (debugMode) Debug.Log("Skill2 Timer Expired: Resetting to Stage 0");
            }
        }
        if (skill3TimeLeft > 0)
        {
            skill3TimeLeft -= Time.deltaTime;
            if (skill3TimeLeft <= 0)
            {
                skill3Stage = 0;
                animator.ResetTrigger(skill3TriggerHash);
                if (debugMode) Debug.Log("Skill3 Timer Expired: Resetting to Stage 0");
            }
        }
    }

    void onDash(InputAction.CallbackContext context)
    {
        // * Handle dash input and initiate dash
        if (context.phase == InputActionPhase.Performed && dashCooldownLeft <= 0)
        {
            audioSource.PlayOneShot(dashSound);
            dashTimeLeft = dashDuration;
            dashCooldownLeft = dashCooldown;
            speed = dashSpeed;
        }
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        // * Handle movement input
        currentMovementInput = context.ReadValue<Vector2>();
        HandleMovement();
    }

    private void HandleMovement()
    {
        // * Calculate and apply movement based on input
        Vector2 isoMovementInput = RotateInput(currentMovementInput, -45);
        currentMovement.x = isoMovementInput.x * speed;
        currentMovement.z = isoMovementInput.y * speed;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    Vector2 RotateInput(Vector2 input, float degrees)
    {
        // * Rotate input vector by a given angle
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = input.x;
        float ty = input.y;
        input.x = (cos * tx) - (sin * ty);
        input.y = (sin * tx) + (cos * ty);

        return input;
    }

    void HandleLookDirection()
    {
        // * Rotate the player to face the mouse position
        Vector3 mousePosition = GetMouseWorldPosition();
        Vector3 directionToLookAt = (mousePosition - transform.position).normalized;
        directionToLookAt.y = 0; // Ensure the character only rotates on the Y axis

        Quaternion targetRotation = Quaternion.LookRotation(directionToLookAt);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f); // Smooth rotation
    }

    Vector3 GetMouseWorldPosition()
    {
        // * Get the mouse position in the world
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }

    public void onAttack()
    {
        // * Trigger attack and play a random attack sound
        scytheDamage.OnAttack();
        int rand = Random.Range(0, attackSound.Length);
        audioSource.PlayOneShot(attackSound[rand]);
    }

    void UpdateAnimatorParameters()
    {
        // * Update animator parameters for movement
        Vector3 movementDirection = characterController.transform.InverseTransformDirection(currentMovement);
        animator.SetFloat("MoveX", movementDirection.x);
        animator.SetFloat("MoveZ", movementDirection.z);
    }

    void onBerserk(InputAction.CallbackContext context)
    {
        // * Handle berserk input and activate berserk mode
        if (context.phase == InputActionPhase.Performed && !isBerserk)
        {
            ActivateBerserk();
        }
    }

    void ActivateBerserk()
    {
        // * Activate berserk mode
        isBerserk = true;
        berserkEndTime = Time.time + berserkDuration;

        dashCooldown = 0.5f;
        speed = normalSpeed * 1.9f;
        animator.SetTrigger(berserkTriggerHash);

        if (berserkCooldownUI != null)
        {
            berserkCooldownUI.StartCooldown(berserkDuration);
        }
    }

    void onSkill1(InputAction.CallbackContext context)
    {
        // * Handle Skill1 input and activate skill
        if (context.phase == InputActionPhase.Performed && !skill1Pressed && !gamePaused)
        {
            skill1Pressed = true;
            skill1Stage = Mathf.Clamp(skill1Stage + 1, 1, 3);
            skill1TimeLeft = skillStageDuration;
            animator.SetTrigger(skill1TriggerHash);
            if (debugMode) Debug.Log("Skill1 Stage: " + skill1Stage);

            StartCoroutine(ResetSkill1Pressed());
        }
    }

    IEnumerator ResetSkill1Pressed()
    {
        // * Reset Skill1 pressed state after a delay
        yield return new WaitForSeconds(0.4f);
        skill1Pressed = false;
    }

    void onSkill2(InputAction.CallbackContext context)
    {
        // * Handle Skill2 input and activate skill
        if (context.phase == InputActionPhase.Performed)
        {
            skill2Stage++;
            skill2TimeLeft = skillStageDuration;
            animator.SetTrigger(skill2TriggerHash);
            if (debugMode) Debug.Log("Skill2 Stage: " + skill2Stage);
        }
    }

    void onSkill3(InputAction.CallbackContext context)
    {
        // * Handle Skill3 input and activate skill
        if (context.phase == InputActionPhase.Performed)
        {
            skill3Stage++;
            skill3TimeLeft = skillStageDuration;
            animator.SetTrigger(skill3TriggerHash);
            if (debugMode) Debug.Log("Skill3 Stage: " + skill3Stage);
        }
    }

    void DeactivateBerserk()
    {
        // * Deactivate berserk mode
        isBerserk = false;
        dashCooldown = 1.5f;
        speed = normalSpeed;
    }

    void OnEnable()
    {
        // * Enabling the PlayerInput
        playerInput.CharacterControls.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // * Disabling the PlayerInput
        playerInput.CharacterControls.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // * Handle actions after the scene has loaded
        if (DataPersistenceManager.instance.isLoading)
        {
            if (debugMode) Debug.Log("triggered onsceneloaded of charactercontrols " + DataPersistenceManager.instance.loadedPlayerPos);
            this.transform.SetPositionAndRotation(DataPersistenceManager.instance.loadedPlayerPos + new Vector3(0, 0.4f, 0), this.transform.rotation);
            DataPersistenceManager.instance.isLoading = false;
        }
    }

    public void LoadData(GameData data)
    {
        // * Load player position from saved data
        this.transform.SetPositionAndRotation(data.playerPos, this.transform.rotation);
    }

    public void SaveData(GameData data)
    {
        // * Save player position to game data
        data.playerPos = this.transform.position;
    }

    void onPause(InputAction.CallbackContext context)
    {
        // * Handle pause input and toggle pause menu
        if (context.performed)
        {
            if (pauseMenu == null)
            {
                pauseMenu = GameObject.Find("PauseMenu")?.GetComponent<Pause>();
                if (pauseMenu == null)
                {
                    Debug.LogError("PauseMenu object not found or Pause component missing during pause action!");
                    return;
                }
            }
            pauseMenu.PressPause();
        }
    }
}

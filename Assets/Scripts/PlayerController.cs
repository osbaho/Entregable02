using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Player Movement Variables ---
    [Header("Movement Settings")]
    [Tooltip("Speed of the player when walking.")]
    public float moveSpeed = 5.0f;
    [Tooltip("Strength of the player's jump.")]
    public float jumpForce = 5.0f;
    [Tooltip("Gravity applied to the player.")]
    public float gravity = -9.81f;
    [Tooltip("Multiplier for a shorter jump when the jump button is released early.")]
    public float lowJumpMultiplier = 2f;
    [Tooltip("Multiplier for a faster fall.")]
    public float fallMultiplier = 2f;

    // --- Mouse Look/Rotation Variables ---
    [Header("Look Settings")]
    [Tooltip("Sensitivity of the mouse for looking left and right.")]
    public float mouseSensitivity = 2.0f;

    [Header("Dependencies")]
    [Tooltip("The transform of the main camera.")]
    public Transform cameraTransform;

    // --- Private Variables ---
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private InputSystem_Actions playerControls;

    // Jump Buffering
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerControls = new InputSystem_Actions();

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        playerControls.Player.Look.performed += OnLookPerformed;
        playerControls.Player.Look.canceled += OnLookCanceled;
        playerControls.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        playerControls.Player.Move.performed -= OnMovePerformed;
        playerControls.Player.Move.canceled -= OnMoveCanceled;
        playerControls.Player.Look.performed -= OnLookPerformed;
        playerControls.Player.Look.canceled -= OnLookCanceled;
        playerControls.Player.Jump.performed -= OnJumpPerformed;
        playerControls.Player.Disable();
    }

    private void Update()
    {
        HandleRotation();
        HandleMovement();
        HandleGravity();

        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    // --- Input System Callbacks ---
    private void OnMovePerformed(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext context) => moveInput = Vector2.zero;
    private void OnLookPerformed(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();
    private void OnLookCanceled(InputAction.CallbackContext context) => lookInput = Vector2.zero;
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    // --- Handlers ---
    private void HandleRotation()
    {
        // We only rotate the player's body left and right. 
        // Cinemachine, with the Input Provider, handles the up/down look.
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        // Get camera vectors and flatten them on the XZ plane
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // Use the original x for right and y for forward, relative to the camera
        Vector3 moveDirection = camRight * moveInput.x + camForward * moveInput.y;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
    
    private void HandleGravity()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded)
        {
            // Reset vertical velocity when grounded.
            // A small negative value helps keep the character grounded.
            if (playerVelocity.y < 0)
            {
                playerVelocity.y = -2f;
            }

            // Check for buffered jump
            if (jumpBufferCounter > 0f)
            {
                playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                jumpBufferCounter = 0f; // Reset buffer
            }
        }
        else // The player is in the air
        {
            // Apply custom gravity for variable jump height and faster fall
            if (playerVelocity.y < 0) // Falling
            {
                playerVelocity.y += gravity * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (playerVelocity.y > 0 && !playerControls.Player.Jump.IsPressed()) // Short hop
            {
                playerVelocity.y += gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
            }

            // Apply base gravity
            playerVelocity.y += gravity * Time.deltaTime;
        }

        // Move the character
        characterController.Move(playerVelocity * Time.deltaTime);
    }
}

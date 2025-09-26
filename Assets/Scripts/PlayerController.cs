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

    // --- Mouse Look/Rotation Variables ---
    [Header("Look Settings")]
    [Tooltip("Sensitivity of the mouse for looking left and right.")]
    public float mouseSensitivity = 2.0f;

    // --- Private Variables ---
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private InputSystem_Actions playerControls;

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
    }

    // --- Input System Callbacks ---
    private void OnMovePerformed(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext context) => moveInput = Vector2.zero;
    private void OnLookPerformed(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();
    private void OnLookCanceled(InputAction.CallbackContext context) => lookInput = Vector2.zero;
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
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
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
    
    private void HandleGravity()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }
}

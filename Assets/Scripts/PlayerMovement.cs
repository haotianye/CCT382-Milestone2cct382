
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float sprintSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Dodge")]
    public float dodgeSpeed = 6f;
    public float dodgeDuration = 0.45f;
    public float dodgeCooldown = 0.8f;

    private CharacterController controller;
    private Animator animator;
    private Transform mainCamera;

    private Vector2 moveInput;
    private float verticalVelocity;

    private bool isSprinting;
    private bool isDodging;

    private float dodgeTimer;
    private float dodgeCooldownTimer;
    private Vector3 dodgeDirection;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    // WASD / Left Stick
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Shift / Gamepad L3
    public void OnSprint(InputValue value)
    {
        // Sprint is read directly from devices in Update.
        // This prevents the running state from getting stuck.
    }

    // Space / Gamepad A
    public void OnJump(InputValue value)
    {
        if (!value.isPressed || isDodging)
            return;

        if (controller.isGrounded)
        {
            verticalVelocity =
                Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    // E / Gamepad B
    public void OnDodge(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (isDodging ||
            dodgeCooldownTimer > 0f ||
            !controller.isGrounded)
            return;

        isDodging = true;
        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;

        // Dodge toward the direction the character faces.
        dodgeDirection = transform.forward;

        if (animator != null)
        {
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Dodge");
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Keyboard sprint: Shift
        bool keyboardSprint =
            Keyboard.current != null &&
            (Keyboard.current.leftShiftKey.isPressed ||
             Keyboard.current.rightShiftKey.isPressed);

        // Gamepad sprint: L3
        bool controllerSprint =
            Gamepad.current != null &&
            Gamepad.current.leftStickButton.isPressed;

        isSprinting = keyboardSprint || controllerSprint;

        // Find the main camera.
        if (mainCamera == null && Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }

        // Camera-relative movement
        Vector3 cameraForward = Vector3.forward;
        Vector3 cameraRight = Vector3.right;

        if (mainCamera != null)
        {
            cameraForward = mainCamera.forward;
            cameraRight = mainCamera.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();
        }

        Vector3 movement =
            cameraForward * moveInput.y +
            cameraRight * moveInput.x;

        movement = Vector3.ClampMagnitude(movement, 1f);

        // Rotate character toward movement direction
        if (!isDodging && movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(movement);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * dt
            );
        }

        // Movement speed
        float currentSpeed =
            isSprinting ? sprintSpeed : walkSpeed;

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * dt;

        // Dodge cooldown
        if (dodgeCooldownTimer > 0f)
        {
            dodgeCooldownTimer -= dt;
        }

        // Horizontal movement
        Vector3 horizontalMovement;

        if (isDodging)
        {
            horizontalMovement = dodgeDirection * dodgeSpeed;

            dodgeTimer -= dt;

            if (dodgeTimer <= 0f)
            {
                isDodging = false;
            }
        }
        else
        {
            horizontalMovement = movement * currentSpeed;
        }

        // Apply movement and gravity
        Vector3 finalMovement = horizontalMovement;
        finalMovement.y = verticalVelocity;

        controller.Move(finalMovement * dt);

        // Animator Blend Tree
        if (animator != null)
        {
            float animationSpeed = 0f;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                animationSpeed = isSprinting ? 1f : 0.5f;
            }

            animator.SetFloat("Speed", animationSpeed);
        }
    }
}

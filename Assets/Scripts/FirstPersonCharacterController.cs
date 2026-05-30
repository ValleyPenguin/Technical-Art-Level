using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonCharacterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float gravity = -15f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 0.08f;
    [SerializeField] private float controllerSensitivity = 130f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.65f, 0f);

    private CharacterController controller;
    private InputActionMap playerActions;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private float verticalVelocity;
    private float cameraPitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (cameraTransform != null)
        {
            cameraTransform.SetParent(transform, true);
            cameraTransform.localPosition = cameraOffset;
        }

        if (inputActions == null)
        {
            Debug.LogError("InputSystem_Actions is not assigned.", this);
            enabled = false;
            return;
        }

        playerActions = inputActions.FindActionMap("Player", true);
        moveAction = playerActions.FindAction("Move", true);
        lookAction = playerActions.FindAction("Look", true);
        jumpAction = playerActions.FindAction("Jump", true);
        sprintAction = playerActions.FindAction("Sprint", true);
    }

    private void OnEnable()
    {
        playerActions?.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        playerActions?.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Move();
        Look();

        if (cameraTransform != null)
        {
            cameraTransform.localPosition = cameraOffset;
        }
    }

    private void Move()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        move = Vector3.ClampMagnitude(move, 1f);

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1f;
        }

        if (controller.isGrounded && jumpAction.WasPressedThisFrame())
        {
            verticalVelocity = jumpPower;
        }

        verticalVelocity += gravity * Time.deltaTime;

        float speed = sprintAction.IsPressed() ? sprintSpeed : walkSpeed;
        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Look()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        if (lookAction.activeControl != null && lookAction.activeControl.device is Gamepad)
        {
            look *= controllerSensitivity * Time.deltaTime;
        }
        else
        {
            look *= mouseSensitivity;
        }

        transform.Rotate(Vector3.up * look.x);

        cameraPitch -= look.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }
}

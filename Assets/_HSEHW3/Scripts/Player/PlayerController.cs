using UnityEngine;
using UnityEngine.InputSystem;

namespace _HSEHW3.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private InputActionReference runAction;
        [SerializeField] private InputActionReference toggleRootMotionAction;
        [SerializeField] private InputActionReference rollAction;
        [SerializeField] private InputActionReference attackAction;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 2.0f;
        [SerializeField] private float runSpeed = 4.5f;
        [SerializeField] private float turnSpeed = 720.0f;

        [Header("Camera")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private bool lockCursor = true;

        [Header("Mode")]
        [SerializeField] private bool useRootMotion;

        [Header("Animator Parameters")]
        [SerializeField] private float dampTime = 0.2f;
        [SerializeField] private string speedParameter = "Speed";

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private InteractionSensor interactionSensor;
        [SerializeField] private Rigidbody targetRigidbody;

        private Vector3 desiredMoveDirection;
        private bool hasMoveInput;
        private bool isRunning;

        private void OnEnable()
        {
            EnableAction(moveAction);
            EnableAction(interactAction);
            EnableAction(runAction);
            EnableAction(toggleRootMotionAction);
            EnableAction(rollAction);
            EnableAction(attackAction);

            interactAction.action.performed += OnInteractPerformed;
            toggleRootMotionAction.action.performed += OnToggleRootMotionPerformed;
            rollAction.action.performed += OnRollPerformed;
            attackAction.action.performed += OnAttackPerformed;

            animator.applyRootMotion = useRootMotion;

            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void OnDisable()
        {
            interactAction.action.performed -= OnInteractPerformed;
            toggleRootMotionAction.action.performed -= OnToggleRootMotionPerformed;
            rollAction.action.performed -= OnRollPerformed;
            attackAction.action.performed -= OnAttackPerformed;

            DisableAction(moveAction);
            DisableAction(interactAction);
            DisableAction(runAction);
            DisableAction(toggleRootMotionAction);
            DisableAction(rollAction);
            DisableAction(attackAction);

            if (lockCursor && Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void Update()
        {
            UpdateInput();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        private void UpdateInput()
        {
            Vector2 moveInput = ReadMoveInput();
            isRunning = IsRunPressed();

            desiredMoveDirection = GetMoveDirection(moveInput);
            hasMoveInput = desiredMoveDirection.sqrMagnitude > 0.0001f;
        }

        private void UpdateAnimator()
        {
            float targetSpeedParam = 0f;
            if (hasMoveInput)
            {
                targetSpeedParam = isRunning ? 1f : 0.5f;
            }

            animator.SetFloat(speedParameter, targetSpeedParam, dampTime, Time.deltaTime);
        }

        private void ApplyMovement()
        {
            Vector3 currentVelocity = targetRigidbody.linearVelocity;
            Vector3 horizontalVelocity = Vector3.zero;

            if (hasMoveInput)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection, Vector3.up);
                Quaternion nextRotation = Quaternion.RotateTowards(
                    targetRigidbody.rotation,
                    targetRotation,
                    turnSpeed * Time.fixedDeltaTime
                );
                targetRigidbody.MoveRotation(nextRotation);
            }

            if (!useRootMotion && hasMoveInput)
            {
                float moveSpeed = isRunning ? runSpeed : walkSpeed;
                horizontalVelocity = desiredMoveDirection * moveSpeed;
            }

            targetRigidbody.linearVelocity = new Vector3(
                horizontalVelocity.x,
                currentVelocity.y,
                horizontalVelocity.z
            );
        }

        private Vector3 GetMoveDirection(Vector2 moveInput)
        {
            Transform movementReference = cameraTransform ? cameraTransform : transform;

            Vector3 forward = movementReference ? movementReference.forward : transform.forward;
            Vector3 right = movementReference ? movementReference.right : transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
            return Vector3.ClampMagnitude(moveDirection, 1f);
        }

        private Vector2 ReadMoveInput()
        {
            return moveAction.action.ReadValue<Vector2>();
        }

        private bool IsRunPressed()
        {
            return runAction.action.IsPressed();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            interactionSensor.TryInteractCurrent();
        }

        private void OnToggleRootMotionPerformed(InputAction.CallbackContext context)
        {
            useRootMotion = !useRootMotion;
            animator.applyRootMotion = useRootMotion;
        }

        private void OnRollPerformed(InputAction.CallbackContext context)
        {
            animator.SetTrigger("Roll");
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            animator.SetTrigger("Attack");
        }

        private static void EnableAction(InputActionReference actionReference)
        {
            actionReference.action.Enable();
        }

        private static void DisableAction(InputActionReference actionReference)
        {
            actionReference.action.Disable();
        }
    }
}

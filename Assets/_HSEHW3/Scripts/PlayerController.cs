using UnityEngine;
using UnityEngine.InputSystem;

namespace _HSEHW3.Scripts
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

            if (interactAction != null && interactAction.action != null)
            {
                interactAction.action.performed += OnInteractPerformed;
            }

            if (toggleRootMotionAction != null && toggleRootMotionAction.action != null)
            {
                toggleRootMotionAction.action.performed += OnToggleRootMotionPerformed;
            }

            if (rollAction != null && rollAction.action != null)
            {
                rollAction.action.performed += OnRollPerformed;
            }

            if (attackAction != null && attackAction.action != null)
            {
                attackAction.action.performed += OnAttackPerformed;
            }

            animator.applyRootMotion = useRootMotion;

            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void OnDisable()
        {
            if (interactAction != null && interactAction.action != null)
            {
                interactAction.action.performed -= OnInteractPerformed;
            }

            if (toggleRootMotionAction != null && toggleRootMotionAction.action != null)
            {
                toggleRootMotionAction.action.performed -= OnToggleRootMotionPerformed;
            }

            if (rollAction != null && rollAction.action != null)
            {
                rollAction.action.performed -= OnRollPerformed;
            }

            if (attackAction != null && attackAction.action != null)
            {
                attackAction.action.performed -= OnAttackPerformed;
            }

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
            if (targetRigidbody == null)
            {
                return;
            }

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
                Vector3 nextPosition = targetRigidbody.position + desiredMoveDirection * (moveSpeed * Time.fixedDeltaTime);
                targetRigidbody.MovePosition(nextPosition);
            }
        }

        private Vector3 GetMoveDirection(Vector2 moveInput)
        {
            Transform movementReference = cameraTransform != null ? cameraTransform : transform;

            Vector3 forward = movementReference != null ? movementReference.forward : transform.forward;
            Vector3 right = movementReference != null ? movementReference.right : transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
            return Vector3.ClampMagnitude(moveDirection, 1f);
        }

        private Vector2 ReadMoveInput()
        {
            if (moveAction == null || moveAction.action == null)
            {
                return Vector2.zero;
            }

            return moveAction.action.ReadValue<Vector2>();
        }

        private bool IsRunPressed()
        {
            if (runAction == null || runAction.action == null)
            {
                return false;
            }

            return runAction.action.IsPressed();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (interactionSensor == null)
            {
                return;
            }

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
            if (actionReference != null && actionReference.action != null)
            {
                actionReference.action.Enable();
            }
        }

        private static void DisableAction(InputActionReference actionReference)
        {
            if (actionReference != null && actionReference.action != null)
            {
                actionReference.action.Disable();
            }
        }
    }
}

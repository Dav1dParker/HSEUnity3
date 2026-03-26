using UnityEngine;
using UnityEngine.InputSystem;

namespace _HSEHW3.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference runAction;
        [SerializeField] private InputActionReference toggleRootMotionAction;
        [SerializeField] private InputActionReference rollAction;
        [SerializeField] private InputActionReference attackAction;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 2.0f;
        [SerializeField] private float runSpeed = 4.5f;
        [SerializeField] private float turnSpeed = 720.0f;
        [SerializeField] private float jumpForce = 6.0f;
        [SerializeField] private float jumpDelay = 0.1f;
        [SerializeField] private float groundCheckDistance = 0.25f;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayers = ~0;

        [Header("Camera")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private bool lockCursor = true;

        [Header("Mode")]
        [SerializeField] private bool useRootMotion;

        [Header("Animator")]
        [SerializeField] private float dampTime = 0.2f;
        [SerializeField] private string speedParameter = "Speed";

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private InteractionSensor interactionSensor;
        [SerializeField] private Rigidbody targetRigidbody;

        private Vector3 _desiredMoveDirection;
        private bool _hasMoveInput;
        private bool _isRunning;
        private bool _jumpRequested;
        private float _jumpTime;

        private void OnEnable()
        {
            EnableAction(moveAction);
            EnableAction(interactAction);
            EnableAction(jumpAction);
            EnableAction(runAction);
            EnableAction(toggleRootMotionAction);
            EnableAction(rollAction);
            EnableAction(attackAction);

            interactAction.action.performed += OnInteractPerformed;
            jumpAction.action.performed += OnJumpPerformed;
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
            jumpAction.action.performed -= OnJumpPerformed;
            toggleRootMotionAction.action.performed -= OnToggleRootMotionPerformed;
            rollAction.action.performed -= OnRollPerformed;
            attackAction.action.performed -= OnAttackPerformed;

            DisableAction(moveAction);
            DisableAction(interactAction);
            DisableAction(jumpAction);
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
            _isRunning = IsRunPressed();

            _desiredMoveDirection = GetMoveDirection(moveInput);
            _hasMoveInput = _desiredMoveDirection.sqrMagnitude > 0.0001f;
        }

        private void UpdateAnimator()
        {
            float targetSpeedParam = 0f;
            if (_hasMoveInput)
            {
                targetSpeedParam = _isRunning ? 1f : 0.5f;
            }

            animator.SetFloat(speedParameter, targetSpeedParam, dampTime, Time.deltaTime);
        }

        private void ApplyMovement()
        {
            Vector3 currentVelocity = targetRigidbody.linearVelocity;
            Vector3 horizontalVelocity = Vector3.zero;

            if (_hasMoveInput)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_desiredMoveDirection, Vector3.up);
                Quaternion nextRotation = Quaternion.RotateTowards(
                    targetRigidbody.rotation,
                    targetRotation,
                    turnSpeed * Time.fixedDeltaTime
                );
                targetRigidbody.MoveRotation(nextRotation);
            }

            if (!useRootMotion && _hasMoveInput)
            {
                float moveSpeed = _isRunning ? runSpeed : walkSpeed;
                horizontalVelocity = _desiredMoveDirection * moveSpeed;
            }

            targetRigidbody.linearVelocity = new Vector3(
                horizontalVelocity.x,
                currentVelocity.y,
                horizontalVelocity.z
            );

            if (_jumpRequested && Time.time >= _jumpTime && IsGrounded())
            {
                Vector3 velocity = targetRigidbody.linearVelocity;
                velocity.y = jumpForce;
                targetRigidbody.linearVelocity = velocity;
                _jumpRequested = false;
            }
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

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _jumpRequested = true;
            _jumpTime = Time.time + jumpDelay;

            if (IsGrounded())
            {
                animator.SetTrigger("Jump");
            }
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

        private bool IsGrounded()
        {
            Vector3 origin = targetRigidbody.worldCenterOfMass + Vector3.up * 0.05f;
            return Physics.SphereCast(
                origin,
                groundCheckRadius,
                Vector3.down,
                out _,
                groundCheckDistance,
                groundLayers,
                QueryTriggerInteraction.Ignore
            );
        }
    }
}

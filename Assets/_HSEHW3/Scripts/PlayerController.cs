using UnityEngine;
using UnityEngine.InputSystem;

namespace _HSEHW3.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference runAction;
        [SerializeField] private InputActionReference toggleRootMotionAction;
        [SerializeField] private InputActionReference rollAction;
        [SerializeField] private InputActionReference attackAction;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 2.0f;
        [SerializeField] private float runSpeed = 4.5f;
        [SerializeField] private float turnSpeed = 720.0f;

        [Header("Camera")]
        [SerializeField] private bool lockCursor = true;

        [Header("Mode")]
        [SerializeField] private bool useRootMotion;

        [Header("Animator Parameters")]
        [SerializeField] private float dampTime = 0.2f;
        [SerializeField] private string speedParameter = "Speed";

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            EnableAction(moveAction);
            EnableAction(runAction);
            EnableAction(toggleRootMotionAction);
            EnableAction(rollAction);
            EnableAction(attackAction);

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
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            Vector2 moveInput = ReadMoveInput();
            bool running = IsRunPressed();

            Vector3 moveDirection = GetMoveDirection(moveInput);
            bool hasInput = moveDirection.sqrMagnitude > 0.0001f;

            if (hasInput)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                );
            }

            float targetSpeedParam = 0f;
            if (hasInput)
            {
                targetSpeedParam = running ? 1f : 0.5f;
            }

            animator.SetFloat(speedParameter, targetSpeedParam, dampTime, Time.deltaTime);

            if (!useRootMotion && hasInput)
            {
                float moveSpeed = running ? runSpeed : walkSpeed;
                transform.position += moveDirection * (moveSpeed * Time.deltaTime);
            }
        }

        private Vector3 GetMoveDirection(Vector2 moveInput)
        {
            Camera activeCamera = Camera.main;
            Transform movementReference = activeCamera != null ? activeCamera.transform : transform;

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

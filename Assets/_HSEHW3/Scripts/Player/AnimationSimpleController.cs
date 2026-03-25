using UnityEngine;
using UnityEngine.InputSystem;

namespace _HSEHW3.Scripts.Player
{
    public class AnimationSimpleController : MonoBehaviour
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

        [Header("Mode")]
        [SerializeField] private bool useRootMotion = false;

        [Header("References")]
        [SerializeField] private Animator animator;

        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
        private static readonly int RollHash = Animator.StringToHash("Roll");
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        private void Awake()
        {
            animator.applyRootMotion = useRootMotion;
        }

        private void OnEnable()
        {
            EnableAction(moveAction);
            EnableAction(runAction);
            EnableAction(toggleRootMotionAction);
            EnableAction(rollAction);
            EnableAction(attackAction);

            toggleRootMotionAction.action.performed += OnToggleRootMotionPerformed;
            rollAction.action.performed += OnRollPerformed;
            attackAction.action.performed += OnAttackPerformed;

            animator.applyRootMotion = useRootMotion;
        }

        private void OnDisable()
        {
            toggleRootMotionAction.action.performed -= OnToggleRootMotionPerformed;
            rollAction.action.performed -= OnRollPerformed;
            attackAction.action.performed -= OnAttackPerformed;

            DisableAction(moveAction);
            DisableAction(runAction);
            DisableAction(toggleRootMotionAction);
            DisableAction(rollAction);
            DisableAction(attackAction);
        }

        private void Update()
        {
            Vector2 moveInput = ReadMoveInput();
            bool running = IsRunPressed();

            Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
            input = Vector3.ClampMagnitude(input, 1f);

            bool hasInput = input.sqrMagnitude > 0.0001f;

            if (hasInput)
            {
                Quaternion targetRotation = Quaternion.LookRotation(input, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                );
            }

            animator.SetBool(IsMovingHash, hasInput);
            animator.SetBool(IsRunningHash, hasInput && running);

            if (!useRootMotion)
            {
                float moveSpeed = running ? runSpeed : walkSpeed;
                transform.position += input * (moveSpeed * Time.deltaTime);
            }
        }

        private Vector2 ReadMoveInput()
        {
            return moveAction.action.ReadValue<Vector2>();
        }

        private bool IsRunPressed()
        {
            return runAction.action.IsPressed();
        }

        private void OnToggleRootMotionPerformed(InputAction.CallbackContext context)
        {
            useRootMotion = !useRootMotion;
            animator.applyRootMotion = useRootMotion;
        }

        private void OnRollPerformed(InputAction.CallbackContext context)
        {
            animator.SetTrigger(RollHash);
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            animator.SetTrigger(AttackHash);
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

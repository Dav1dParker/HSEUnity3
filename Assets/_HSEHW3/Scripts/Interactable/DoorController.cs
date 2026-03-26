using UnityEngine;

namespace _HSEHW3.Scripts.Interactable
{
    public class DoorController : MonoBehaviour, IInteractable
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Collider blockingCollider;
        [SerializeField] private string promptText = "E";
        [SerializeField] private string openTrigger = "Open";
        [SerializeField] private string closeTrigger = "Close";
        [SerializeField] private bool startsOpen;
        [SerializeField] private bool disableColliderWhenOpen;

        private bool _isOpen;

        public string PromptText => promptText;
        public bool CanInteract => enabled && gameObject.activeInHierarchy;
        public bool IsOpen => _isOpen;

        private void Awake()
        {
            _isOpen = !startsOpen;
            SetOpen(startsOpen, true);
        }

        public void Interact()
        {
            Toggle();
        }

        public void Toggle()
        {
            SetOpen(!_isOpen);
        }

        public void Close()
        {
            SetOpen(false);
        }

        public void Open()
        {
            SetOpen(true);
        }

        public void SetOpen(bool value)
        {
            SetOpen(value, false);
        }

        private void SetOpen(bool value, bool instant)
        {
            if (_isOpen == value)
            {
                return;
            }

            _isOpen = value;

            if (instant)
            {
                animator.Play(_isOpen ? "OpenIdle" : "ClosedIdle", 0, 0f);
                animator.Update(0f);
            }
            else
            {
                animator.ResetTrigger(openTrigger);
                animator.ResetTrigger(closeTrigger);
                animator.SetTrigger(_isOpen ? openTrigger : closeTrigger);
            }

            if (disableColliderWhenOpen)
            {
                blockingCollider.enabled = !_isOpen;
            }
        }
    }
}

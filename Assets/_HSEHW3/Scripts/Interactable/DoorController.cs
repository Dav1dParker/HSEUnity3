using UnityEngine;

namespace _HSEHW3.Scripts.Interactable
{
    public class DoorController : MonoBehaviour, IInteractable
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Collider blockingCollider;
        [SerializeField] private string promptText = "E";
        [SerializeField] private string openParameter = "IsOpen";
        [SerializeField] private bool startsOpen;
        [SerializeField] private bool disableColliderWhenOpen;

        private bool isOpen;

        public string PromptText => promptText;
        public bool CanInteract => enabled && gameObject.activeInHierarchy;
        public bool IsOpen => isOpen;

        private void Awake()
        {
            SetOpen(startsOpen, true);
        }

        public void Interact()
        {
            Toggle();
        }

        public void Toggle()
        {
            SetOpen(!isOpen);
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
            isOpen = value;

            animator.SetBool(openParameter, isOpen);
            if (instant)
            {
                animator.Update(0f);
            }

            if (disableColliderWhenOpen)
            {
                blockingCollider.enabled = !isOpen;
            }
        }
    }
}

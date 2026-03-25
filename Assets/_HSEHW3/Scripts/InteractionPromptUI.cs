using TMPro;
using UnityEngine;

namespace _HSEHW3.Scripts
{
    public class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private InteractionSensor interactionSensor;
        [SerializeField] private TMP_Text promptLabel;

        private void Awake()
        {
            RefreshPrompt();
        }

        private void OnEnable()
        {
            if (interactionSensor != null)
            {
                interactionSensor.CurrentInteractableChanged += OnCurrentInteractableChanged;
            }

            RefreshPrompt();
        }

        private void OnDisable()
        {
            if (interactionSensor != null)
            {
                interactionSensor.CurrentInteractableChanged -= OnCurrentInteractableChanged;
            }
        }

        private void OnCurrentInteractableChanged(IInteractable interactable)
        {
            RefreshPrompt();
        }

        private void RefreshPrompt()
        {
            if (promptLabel == null)
            {
                return;
            }

            IInteractable interactable = interactionSensor != null ? interactionSensor.CurrentInteractable : null;
            bool shouldShow = interactable != null && interactable.CanInteract;

            promptLabel.text = shouldShow ? interactable.PromptText : string.Empty;
            promptLabel.gameObject.SetActive(shouldShow);
        }
    }
}

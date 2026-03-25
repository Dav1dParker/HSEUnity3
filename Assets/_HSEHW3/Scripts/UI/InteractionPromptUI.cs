using _HSEHW3.Scripts.Interactable;
using _HSEHW3.Scripts.Player;
using TMPro;
using UnityEngine;

namespace _HSEHW3.Scripts.UI
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
            interactionSensor.CurrentInteractableChanged += OnCurrentInteractableChanged;
            RefreshPrompt();
        }

        private void OnDisable()
        {
            interactionSensor.CurrentInteractableChanged -= OnCurrentInteractableChanged;
        }

        private void OnCurrentInteractableChanged(IInteractable interactable)
        {
            RefreshPrompt();
        }

        private void RefreshPrompt()
        {
            IInteractable interactable = interactionSensor.CurrentInteractable;
            bool shouldShow = interactable != null && interactable.CanInteract;

            promptLabel.text = shouldShow ? interactable.PromptText : string.Empty;
            promptLabel.gameObject.SetActive(shouldShow);
        }
    }
}

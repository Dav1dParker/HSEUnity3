using System;
using System.Collections.Generic;
using _HSEHW3.Scripts.Interactable;
using UnityEngine;

namespace _HSEHW3.Scripts.Player
{
    [RequireComponent(typeof(Collider))]
    public class InteractionSensor : MonoBehaviour
    {
        public event Action<IInteractable> CurrentInteractableChanged;

        public IInteractable CurrentInteractable { get; private set; }

        private readonly List<IInteractable> _interactables = new List<IInteractable>();

        private void OnTriggerEnter(Collider other)
        {
            IInteractable interactable = FindInteractable(other);
            if (interactable == null || _interactables.Contains(interactable))
            {
                return;
            }

            _interactables.Add(interactable);
            RefreshCurrentInteractable();
        }

        private void OnTriggerExit(Collider other)
        {
            IInteractable interactable = FindInteractable(other);
            if (interactable == null)
            {
                return;
            }

            if (_interactables.Remove(interactable))
            {
                RefreshCurrentInteractable();
            }
        }

        public bool TryInteractCurrent()
        {
            RefreshCurrentInteractable();
            if (CurrentInteractable == null || !CurrentInteractable.CanInteract)
            {
                return false;
            }

            CurrentInteractable.Interact();
            RefreshCurrentInteractable();
            return true;
        }

        private void RefreshCurrentInteractable()
        {
            _interactables.RemoveAll(item => item == null);

            IInteractable nextInteractable = null;
            float bestDistance = float.MaxValue;

            foreach (IInteractable interactable in _interactables)
            {
                if (interactable == null || !interactable.CanInteract)
                {
                    continue;
                }

                MonoBehaviour behaviour = interactable as MonoBehaviour;
                if (behaviour == null)
                {
                    continue;
                }

                float distance = (behaviour.transform.position - transform.position).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nextInteractable = interactable;
                }
            }

            if (!ReferenceEquals(CurrentInteractable, nextInteractable))
            {
                CurrentInteractable = nextInteractable;
                CurrentInteractableChanged?.Invoke(CurrentInteractable);
            }
        }

        private static IInteractable FindInteractable(Collider other)
        {
            MonoBehaviour[] behaviours = other.GetComponentsInParent<MonoBehaviour>(true);
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IInteractable interactable)
                {
                    return interactable;
                }
            }

            return null;
        }
    }
}

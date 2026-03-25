using _HSEHW3.Scripts.Player;
using UnityEngine;

namespace _HSEHW3.Scripts.Interactable
{
    public class TrapFloor : MonoBehaviour
    {
        [SerializeField] private bool isTrap = true;
        [SerializeField] private DoorController targetDoor;
        [SerializeField] private Collider[] collidersToDisable;
        [SerializeField] private Renderer[] renderersToDisable;

        private bool hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered || !other.CompareTag("Player"))
            {
                return;
            }

            hasTriggered = true;

            targetDoor.Close();
            targetDoor.enabled = false;

            if (isTrap)
            {
                FunnyFall funnyFall = other.GetComponentInParent<FunnyFall>();
                if (funnyFall != null)
                {
                    funnyFall.ActivateFall();
                }
            }

            for (int i = 0; i < collidersToDisable.Length; i++)
            {
                collidersToDisable[i].enabled = false;
            }

            for (int i = 0; i < renderersToDisable.Length; i++)
            {
                renderersToDisable[i].enabled = false;
            }
        }
    }
}

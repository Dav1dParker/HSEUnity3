using UnityEngine;

namespace _HSEHW3.Scripts
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

            if (targetDoor != null)
            {
                targetDoor.Close();
                targetDoor.enabled = false;
            }

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
                if (collidersToDisable[i] != null)
                {
                    collidersToDisable[i].enabled = false;
                }
            }

            for (int i = 0; i < renderersToDisable.Length; i++)
            {
                if (renderersToDisable[i] != null)
                {
                    renderersToDisable[i].enabled = false;
                }
            }
        }
    }
}

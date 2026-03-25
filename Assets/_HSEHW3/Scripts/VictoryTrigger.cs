using UnityEngine;

namespace _HSEHW3.Scripts
{
    public class VictoryTrigger : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleSystems = new ParticleSystem[3];
        [SerializeField] private float quitDelay = 5f;

        private bool hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered || !other.CompareTag("Player"))
            {
                return;
            }

            hasTriggered = true;

            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
            }

            if (quitDelay >= 0f)
            {
                Invoke(nameof(QuitGame), quitDelay);
            }
        }

        private static void QuitGame()
        {
            Application.Quit();
        }
    }
}

using UnityEngine;

namespace _HSEHW3.Scripts
{
    public class VictoryTrigger : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleSystems = new ParticleSystem[3];
        [SerializeField] private float quitDelay = 5f;

        private bool _hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (_hasTriggered || !other.CompareTag("Player"))
            {
                return;
            }

            _hasTriggered = true;

            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
            }

            if (quitDelay >= 0f)
            {
                Invoke(nameof(QuitGame), quitDelay);
            }
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}

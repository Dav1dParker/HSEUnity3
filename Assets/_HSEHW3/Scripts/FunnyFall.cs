using UnityEngine;
using UnityEngine.SceneManagement;

namespace _HSEHW3.Scripts
{
    public class FunnyFall : MonoBehaviour
    {
        private const float MinTorqueImpulse = 6f;
        private const float MaxTorqueImpulse = 12f;

        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody targetRigidbody;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float restartDelay = 2f;

        private bool hasActivated;

        public void ActivateFall()
        {
            if (hasActivated)
            {
                return;
            }

            hasActivated = true;

            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
                animator.enabled = false;
            }

            if (playerController != null)
            {
                playerController.enabled = false;
            }

            if (targetRigidbody != null)
            {
                targetRigidbody.isKinematic = false;
                targetRigidbody.useGravity = true;
                targetRigidbody.constraints = RigidbodyConstraints.None;
                targetRigidbody.AddTorque(GetRandomTorqueImpulse(), ForceMode.Impulse);
            }

            if (restartDelay >= 0f)
            {
                Invoke(nameof(ReloadActiveScene), restartDelay);
            }
        }

        private void ReloadActiveScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.buildIndex);
        }

        private static Vector3 GetRandomTorqueImpulse()
        {
            Vector3 axis = Random.onUnitSphere;
            axis.y = Mathf.Abs(axis.y) + 0.35f;
            axis.Normalize();

            float magnitude = Random.Range(MinTorqueImpulse, MaxTorqueImpulse);
            return axis * magnitude;
        }
    }
}

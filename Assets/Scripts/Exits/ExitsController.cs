using Managers;
using UnityEngine;

namespace Exits
{
    public class ExitsController : MonoBehaviour
    {
        // === Public ===
        public ExitsManager.DirectionType exitDirection;

        // === Private ===
        private Collider _collider;
        private MeshRenderer _renderer;

        // === Unity Methods ===
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _renderer = GetComponent<MeshRenderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Avisar al GameManager del uso de la salida
                ExitsManager.Instance.lastExitUsed = exitDirection;
                GameManager.Instance.HandleExitUsed(exitDirection);
            }
        }

        // === Public Methods ===
        public void ActivateExit()
        {
            _collider.enabled = true;
            _renderer.enabled = true;
        }

        public void DeactivateExit()
        {
            _collider.enabled = false;
            _renderer.enabled = false;
        }
    }
}
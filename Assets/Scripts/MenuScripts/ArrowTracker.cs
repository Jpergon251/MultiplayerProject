using UnityEngine;

namespace MenuScripts
{
    public class ArrowTracker : MonoBehaviour
    {
        public Transform target;

        private Camera mainCamera;
        private RectTransform canvasRect;

        private void Start()
        {
            mainCamera = Camera.main;
            canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (target == null) return;

            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

            // Si está dentro de la pantalla, muévelo hacia el borde
            if (screenPos.z > 0)
            {
                screenPos.x = Mathf.Clamp(screenPos.x, 50, Screen.width - 50);
                screenPos.y = Mathf.Clamp(screenPos.y, 50, Screen.height - 50);
            }

            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.position = screenPos;

            // Apunta hacia la puerta
            Vector3 dir = (target.position - mainCamera.transform.position).normalized;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, -angle);
        }
    }
}
using UnityEngine;

namespace MenuScripts
{
    public class ArrowTracker : MonoBehaviour
    {
        public Transform target;

        private Camera mainCamera;
        private RectTransform canvasRect;
        private RectTransform rectTransform;

        private void Start()
        {
            mainCamera = Camera.main;
            canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (target == null) return;

            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

            // Verificar si está al frente de la cámara
            if (screenPos.z > 0)
            {
                // Clampeamos para mantenerlo dentro de la pantalla (con márgenes)
                screenPos.x = Mathf.Clamp(screenPos.x, 50, Screen.width - 50);
                screenPos.y = Mathf.Clamp(screenPos.y, 50, Screen.height - 50);

                // Convertir a coordenadas locales del Canvas
                Vector2 localPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPos, mainCamera, out localPos);

                rectTransform.anchoredPosition = localPos;
                rectTransform.gameObject.SetActive(true);
            }
            else
            {
                rectTransform.gameObject.SetActive(false);
            }

            // Calcular ángulo hacia el target en el plano horizontal
            Vector3 direction = (target.position - mainCamera.transform.position).normalized;
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            rectTransform.localRotation = Quaternion.Euler(0, 0, -angle);
            
        }
    }
}
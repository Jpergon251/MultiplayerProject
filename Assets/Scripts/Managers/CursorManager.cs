using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance;

        public Texture2D cursorTexture;
        public float scaleFactor = 1.5f;

        private Vector2 hotspot;
        private Texture2D scaledCursor;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupCursor();

            // Reaplicar cursor cada vez que cambie la escena
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void SetupCursor()
        {
            if (cursorTexture == null) return;

            // Escalar textura
            int newWidth = (int)(cursorTexture.width * scaleFactor);
            int newHeight = (int)(cursorTexture.height * scaleFactor);

            scaledCursor = new Texture2D(newWidth, newHeight);
            Graphics.ConvertTexture(cursorTexture, scaledCursor);

            hotspot = new Vector2(scaledCursor.width / 2f, scaledCursor.height / 2f);

            Cursor.SetCursor(scaledCursor, hotspot, CursorMode.ForceSoftware);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetupCursor(); // Vuelve a aplicar el cursor
        }
    }
}
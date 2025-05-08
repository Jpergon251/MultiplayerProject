
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Managers
{
    public class CursorManager : MonoBehaviour
    {
        private static CursorManager Instance;
        
        public Sprite menuCursor;
        public Sprite inGameCursor;

        private GameObject _cursorContainer;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void SetupCursor(Sprite cursorImage)
        {
            _cursorContainer.GetComponent<Image>().sprite = cursorImage;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            _cursorContainer.transform.SetAsLastSibling();
            // Debug.Log(_cursorContainer.GetComponent<Image>().sprite);
        }

        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            
            
            if (scene.name == "StartMenu")
            {
                
                _cursorContainer = GameObject.FindGameObjectWithTag("Cursor");
                   
                SetupCursor(menuCursor);
            }else
            {
                _cursorContainer = GameObject.FindGameObjectWithTag("Cursor");
                SetupCursor(inGameCursor);
            }
            
        }

        private void Update()
        {
            if (_cursorContainer != null)
            {
                RectTransform rectTransform = _cursorContainer.GetComponent<RectTransform>();
                Canvas canvas = _cursorContainer.GetComponentInParent<Canvas>();

                if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
                {
                    Vector2 worldPos;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        rectTransform,
                        Input.mousePosition,
                        canvas.worldCamera,
                        out Vector3 globalMousePos
                    );
                    _cursorContainer.transform.position = globalMousePos;
                }
                else
                {
                    _cursorContainer.transform.position = Input.mousePosition;
                }
            }
        }
    }
}
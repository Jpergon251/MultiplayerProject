using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PlayerScripts
{
    public class PlayerControllerMenu : MonoBehaviour
    {
        [SerializeField] private Scene sceneToLoad;

        public void StartGame(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
            
                // Debug.Log("Iniciando juego desde el menú");
                SceneManager.LoadScene($"GameScene");
                
            }
            
        }
    }
}

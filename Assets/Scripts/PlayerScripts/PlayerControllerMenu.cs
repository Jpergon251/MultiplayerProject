using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PlayerScripts
{
    public class PlayerControllerMenu : MonoBehaviour
    {
        public void StartGame(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
            
                // Debug.Log("Iniciando juego desde el menú");
                SceneManager.LoadSceneAsync("GameScene");
                
            }
            
        }
    }
}

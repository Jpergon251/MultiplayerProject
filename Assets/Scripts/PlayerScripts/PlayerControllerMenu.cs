using System;
using System.Linq;
using MenuScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PlayerScripts
{
    public class PlayerControllerMenu : MonoBehaviour
    {
        public GameObject startMenu;
        public GameObject mainMenu;

        private void Start()
        {
            startMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "StartMenu");
            mainMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "MainMenu");
        }

        public void StartGame(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
            
                // Debug.Log("Iniciando juego desde el menú");
                
                startMenu.SetActive(false);
                mainMenu.SetActive(true);
            }
            
        }
    }
}

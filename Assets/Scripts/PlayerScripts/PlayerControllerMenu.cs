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
        private AudioSettingsManager audioSettingsManager;
        private VideoSettingsManager videoSettingsManager;
        private ControlsSettingsManager controlsSettingsManager;
        private void Start()
        {
            startMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "StartMenu");
            mainMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "MainMenu");
            audioSettingsManager = Resources.FindObjectsOfTypeAll<AudioSettingsManager>().FirstOrDefault();
            videoSettingsManager = Resources.FindObjectsOfTypeAll<VideoSettingsManager>().FirstOrDefault();
            controlsSettingsManager = Resources.FindObjectsOfTypeAll<ControlsSettingsManager>().FirstOrDefault();
            audioSettingsManager.Initialize();
            videoSettingsManager.Initialize();
            Time.timeScale = 1;
        }

        public void StartGame(InputAction.CallbackContext context)
        {
            if (context.performed && startMenu.activeSelf)
            {
            
                // Debug.Log("Iniciando juego desde el menú");
                
                startMenu.SetActive(false);
                mainMenu.SetActive(true);
            }
            
        }
    }
}

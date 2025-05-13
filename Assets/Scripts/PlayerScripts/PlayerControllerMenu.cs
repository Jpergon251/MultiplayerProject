using System;
using System.Linq;
using MenuScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Random = UnityEngine.Random; // Para el uso de TextMeshPro (si deseas mostrar el nombre en la UI)

namespace PlayerScripts
{
    public class PlayerControllerMenu : MonoBehaviour
    {
        public GameObject startMenu;
        public GameObject mainMenu;
        private AudioSettingsManager audioSettingsManager;
        private VideoSettingsManager videoSettingsManager;
        private ControlsSettingsManager controlsSettingsManager;
        
        public static PlayerControllerMenu Instance;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

        }

        private async void Start()
        {
            // Buscar las referencias de los menús y los gestores
            startMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "StartMenu");
            mainMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "MainMenu");
            audioSettingsManager = Resources.FindObjectsOfTypeAll<AudioSettingsManager>().FirstOrDefault();
            videoSettingsManager = Resources.FindObjectsOfTypeAll<VideoSettingsManager>().FirstOrDefault();
            controlsSettingsManager = Resources.FindObjectsOfTypeAll<ControlsSettingsManager>().FirstOrDefault();
            
            // Inicializar configuraciones de audio, video y controles
            audioSettingsManager.Initialize();
            videoSettingsManager.Initialize();
            
            // Activar el tiempo normal
            Time.timeScale = 1;
            
            
        }

        public void StartGame(InputAction.CallbackContext context)
        {
            if (context.performed && startMenu.activeSelf)
            {
                // GeneratePlayerName();
                // Activar el MainMenu y desactivar el StartMenu
                startMenu.SetActive(false);
                mainMenu.SetActive(true);
            }
        }

        // Método para generar un nombre aleatorio para el jugador
        /*private void GeneratePlayerName()
        {
            // Generar un nombre aleatorio para el jugador
            generatedName = "Player_" + Random.Range(1000, 9999);
            // Debug.Log(generatedName);
            PlayerPrefs.SetString("PlayerName", generatedName);
            PlayerPrefs.Save(); // Guarda inmediatamente
            
        }*/

    }
}
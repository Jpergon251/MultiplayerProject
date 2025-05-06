using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.ProBuilder; // Solo si usas TMP_Dropdown

namespace MenuScripts
{
    public class VideoSettingsManager : MonoBehaviour
    {
        [Header("V-Sync Settings")]
        public Toggle vSyncToggle;
        public TMP_Text vSyncText;
        
        [Header("FPS Settings")]
        public TMP_Dropdown fpsDropdown; // O Dropdown si usas el estándar
        public TMP_Text fpsText;
        private readonly int[] fpsOptions = { 30, 60, 120, 144, 240, -1 }; // -1 para sin límite

        [Header("ScreenMode Settings")]
        public TMP_Text screenModeText;
        public TMP_Dropdown screenModeDropdown;
        
        [Header("Resolution Settings")]
        public TMP_Text resolutionText;
        public TMP_Dropdown resolutionDropdown;
        private readonly Vector2Int[] resolutionOptions = new Vector2Int[]
        {
            new Vector2Int(2560, 1920),
            new Vector2Int(2048, 1536),
            new Vector2Int(1600, 1200),
            new Vector2Int(1400, 1050),
            new Vector2Int(1024, 768),
            new Vector2Int(800, 600),
            new Vector2Int(640, 480)
        };
        private List<Vector2Int> filteredResolutionOptions;
        
        public float deltaTime;
        private void Start()
        {
            // Obtener resolución máxima del monitor
            Vector2Int maxResolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);

            // Filtrar resoluciones permitidas
            filteredResolutionOptions = resolutionOptions
                .Where(res => res.x <= maxResolution.x && res.y <= maxResolution.y)
                .ToList();

            // Limpiar y aplicar las resoluciones filtradas
            resolutionDropdown.ClearOptions();
            var resolutionLabels = filteredResolutionOptions.Select(res => $"{res.x} x {res.y}").ToList();
            resolutionDropdown.AddOptions(resolutionLabels);

            // Detectar y seleccionar la resolución actual
            Vector2Int current = new Vector2Int(Screen.width, Screen.height);
            int selectedIndex = filteredResolutionOptions.FindIndex(r => r == current);
            if (selectedIndex == -1) selectedIndex = 0;

            resolutionDropdown.value = selectedIndex;

            // Continuar como antes...
            // Sincronizar modos de pantalla y otros ajustes

            FullScreenMode currentMode = Screen.fullScreenMode;
            int screenModeIndex = currentMode switch
            {
                
                FullScreenMode.FullScreenWindow => 0,
                FullScreenMode.Windowed => 1,
                _ => 0
            };
            screenModeDropdown.value = screenModeIndex;

            int currentFPS = Application.targetFrameRate;
            int index = Array.IndexOf(fpsOptions, currentFPS);
            if (index == -1) index = fpsOptions.Length - 1;

            fpsDropdown.value = index;

            vSyncText.text = vSyncToggle.isOn ? "VSync\n ON" : "VSync\n OFF";
            screenModeText.text = $"ScreenMode \n{Screen.fullScreenMode}";
            resolutionText.text = $"Resolution \n{resolutionLabels[selectedIndex]}";

            // Listeners
            vSyncToggle.onValueChanged.AddListener(SetVSync);
            fpsDropdown.onValueChanged.AddListener(SetFramerate);
            screenModeDropdown.onValueChanged.AddListener(SetScreenMode);
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        private void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = $"FPS \n{Mathf.Ceil(fps).ToString()}";
            // Debug.Log(Application.targetFrameRate);
        }

        public void SetVSync(bool isEnabled)
        {
            QualitySettings.vSyncCount = isEnabled ? 1 : 0;

            if (isEnabled)
            {
                vSyncText.text = vSyncToggle.isOn ? "VSync\n ON" : "VSync\n OFF";
                fpsDropdown.interactable = false;
                Application.targetFrameRate = -1; // Deja que VSync controle los FPS
                // Debug.Log("VSync activado, ignorando limitador de FPS.");
            }
            else
            {
                vSyncText.text = vSyncToggle.isOn ? "VSync\n ON" : "VSync\n OFF";
                fpsDropdown.interactable = true;
                SetFramerate(fpsDropdown.value); // Reaplica el valor del dropdown
                
            }
        }

        public void SetFramerate(int dropdownIndex)
        {
            int targetFPS = fpsOptions[dropdownIndex];
            Application.targetFrameRate = targetFPS;
            // Debug.Log("FPS target: " + (targetFPS <= 0 ? "Sin límite" : targetFPS.ToString()));
        }
        
        public void SetScreenMode(int index)
        {
            switch (index)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                default:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
            }

            screenModeText.text = $"ScreenMode \n{Screen.fullScreenMode.ToString()}";
        }
        
        public void SetResolution(int index)
        {
            var res = filteredResolutionOptions[index];
            Screen.SetResolution(res.x, res.y, Screen.fullScreenMode);
            resolutionText.text = $"Resolution \n{res.x} x {res.y}";
        }
    }
}
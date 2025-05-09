using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MenuScripts
{
    public class VideoSettingsManager : MonoBehaviour
    {
        [Header("V-Sync Settings")]
        public Toggle vSyncToggle;
        public TMP_Text vSyncText;

        [Header("FPS Settings")]
        public TMP_Dropdown fpsDropdown;
        public TMP_Text fpsText;
        private readonly int[] fpsOptions = { 30, 60, 120, 144, 240, -1 };

        [Header("ScreenMode Settings")]
        public TMP_Text screenModeText;
        public TMP_Dropdown screenModeDropdown;

        [Header("Resolution Settings")]
        public TMP_Text resolutionText;
        public TMP_Dropdown resolutionDropdown;
        public Canvas playerCanvas;

        private List<Vector2Int> filteredResolutionOptions;
        private static List<Vector2Int> availableResolutions;
        private static bool initialized = false;
        public float deltaTime;

        public void Initialize()
        {
            if (playerCanvas == null)
                playerCanvas = FindObjectOfType<Canvas>();

            if (playerCanvas.renderMode != RenderMode.ScreenSpaceCamera)
                playerCanvas.renderMode = RenderMode.ScreenSpaceCamera;

            if (!initialized)
            {
                availableResolutions = Screen.resolutions
                    .Select(res => new Vector2Int(res.width, res.height))
                    .Distinct()
                    .Where(res =>
                    {
                        float aspect = (float)res.x / res.y;
                        return Mathf.Abs(aspect - 4f / 3f) < 0.01f;
                    })
                    .OrderByDescending(res => res.x)
                    .ToList();

                initialized = true;
            }

            filteredResolutionOptions = new List<Vector2Int>(availableResolutions);

            resolutionDropdown.ClearOptions();
            var resolutionLabels = filteredResolutionOptions
                .Select(res => $"{res.x} x {res.y}")
                .ToList();
            resolutionDropdown.AddOptions(resolutionLabels);

            Vector2Int current = new Vector2Int(Screen.width, Screen.height);
            int selectedIndex = filteredResolutionOptions.FindIndex(r => r == current);
            if (selectedIndex == -1) selectedIndex = 0;
            resolutionDropdown.value = selectedIndex;

            screenModeDropdown.value = Screen.fullScreenMode switch
            {
                FullScreenMode.FullScreenWindow => 0,
                FullScreenMode.Windowed => 1,
                _ => 0
            };

            int fpsIndex = Array.IndexOf(fpsOptions, Application.targetFrameRate);
            if (fpsIndex == -1) fpsIndex = fpsOptions.Length - 1;
            fpsDropdown.value = fpsIndex;

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
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = $"FPS \n{Mathf.Ceil(fps)}";
        }

        public void SetVSync(bool isEnabled)
        {
            QualitySettings.vSyncCount = isEnabled ? 1 : 0;
            vSyncText.text = isEnabled ? "VSync\n ON" : "VSync\n OFF";
            fpsDropdown.interactable = !isEnabled;
            if (isEnabled)
                Application.targetFrameRate = -1;
            else
                SetFramerate(fpsDropdown.value);
        }

        public void SetFramerate(int dropdownIndex)
        {
            Application.targetFrameRate = fpsOptions[dropdownIndex];
        }

        public void SetScreenMode(int index)
        {
            Screen.fullScreenMode = index switch
            {
                0 => FullScreenMode.FullScreenWindow,
                1 => FullScreenMode.Windowed,
                _ => FullScreenMode.FullScreenWindow
            };

            screenModeText.text = $"ScreenMode \n{Screen.fullScreenMode}";
        }

        public void SetResolution(int index)
        {
            var res = filteredResolutionOptions[index];
            Screen.SetResolution(res.x, res.y, Screen.fullScreenMode);
            resolutionText.text = $"Resolution \n{res.x} x {res.y}";
        }
    }
}

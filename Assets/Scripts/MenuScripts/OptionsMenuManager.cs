using System.Linq;
using Mirror.BouncyCastle.Utilities.Encoders;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts
{
    public class OptionsMenuManager : MonoBehaviour
    {
        public GameObject audioPanel;
        public GameObject videoPanel;
        public GameObject controlsPanel;

        public Button audioButton;
        public Button videoButton;
        public Button controlsButton;
        private void Start()
        {
            var allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            
            audioPanel = allGameObjects.FirstOrDefault(go => go.name == "VolumePanel");
            videoPanel = allGameObjects.FirstOrDefault(go => go.name == "VideoPanel");
            controlsPanel = allGameObjects.FirstOrDefault(go => go.name == "ControlsPanel");
            
            ShowVideoPanel(); // Mostrar panel por defecto
        }

        public void ShowAudioPanel()
        {
            ShowPanel(audioPanel);
            HighlightTab(audioButton);
        }

        public void ShowVideoPanel()
        {
            ShowPanel(videoPanel);
            HighlightTab(videoButton);
        }

        public void ShowControlsPanel()
        {
            ShowPanel(controlsPanel);
            HighlightTab(controlsButton);
        }

        private void ShowPanel(GameObject panelToShow)
        {
            audioPanel.SetActive(panelToShow == audioPanel);
            videoPanel.SetActive(panelToShow == videoPanel);
            controlsPanel.SetActive(panelToShow == controlsPanel);
        }
        
        private void HighlightTab(Button activeButton)
        {
            // Crear copias independientes de los bloques de color
            ColorBlock audioColors = audioButton.colors;
            ColorBlock videoColors = videoButton.colors;
            ColorBlock controlsColors = controlsButton.colors;

            // Resetear colores a su estado base
            audioColors.normalColor = Color.white;
            videoColors.normalColor = Color.white;
            controlsColors.normalColor = Color.white;

            audioButton.colors = audioColors;
            videoButton.colors = videoColors;
            controlsButton.colors = controlsColors;

            // Aplicar color al botón activo
            ColorBlock activeColors = activeButton.colors;
            activeColors.normalColor = ColorHelper.HexToColor("#A3B3E2"); // Puedes usar un fallback
            activeButton.colors = activeColors;
        }
    }
    public static class ColorHelper
    {
        public static Color HexToColor(string hex)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hex, out color))
                return color;
            return default;
        }
    }
}
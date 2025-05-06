using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MenuScripts
{
    public class ControlsSettingsManager : MonoBehaviour
    {
        [Header("Sensitivity")]
        public Slider sensitivitySlider;
        public TMP_Text sensitivityValueText;

        [Header("Input Mode")]
        public TMP_Dropdown inputModeDropdown;
        public Image keyboardInputModeImage;
        public Image controllerInputModeImage;

        private Color deactivatedColor = Color.red;
        private Color activatedColor = Color.green;
        private void Start()
        {
            UpdateSensibility();
            UpdateInputMode();
        }

        public void UpdateSensibility()
        {
            

            // Actualiza texto si está asignado
            if (sensitivityValueText != null)
            {
                sensitivityValueText.text = sensitivitySlider.value.ToString("0.0");
                
            }

            
        }

        public void UpdateInputMode()
        {
            if (inputModeDropdown.value == 0)
            {
                keyboardInputModeImage.color = activatedColor;
                controllerInputModeImage.color = deactivatedColor;
            }
            else
            {
                keyboardInputModeImage.color = deactivatedColor;
                controllerInputModeImage.color = activatedColor;
            }
            // Debug.Log(inputModeDropdown.options[inputModeDropdown.value].text);
        }
    }
}
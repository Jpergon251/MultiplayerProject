using System;
using Music;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MenuScripts
{
    public class AudioSettingsManager : MonoBehaviour
    {
        private MusicPlayer musicPlayer;

        [Header("Master Volume")]
        public TMP_Text masterValueText;
        public Slider masterVolumeSlider;

        [Header("SFX Volume")]
        public TMP_Text sfxValueText;
        public Slider sfxVolumeSlider;

        [Header("Music Volume")]
        public TMP_Text musicValueText;
        public Slider musicVolumeSlider;

        private void Start()
        {
            musicPlayer = FindObjectOfType<MusicPlayer>();

            masterVolumeSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXSliderChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicSliderChanged);

            // Inicialización: master a 10, sfx y music a 5
            masterVolumeSlider.value = 10;
            sfxVolumeSlider.value = 5;
            musicVolumeSlider.value = 5;

            // Aplicar valores iniciales
            OnMasterSliderChanged(10);
            OnSFXSliderChanged(5);
            OnMusicSliderChanged(5);
        }

        private float SliderToVolume(int sliderValue)
        {
            return Mathf.Clamp(sliderValue / 10f, 0.0001f, 1f);
        }

        private void OnMasterSliderChanged(float value)
        {
            int intVal = Mathf.RoundToInt(value);
            masterValueText.text = $"Master: {intVal.ToString()}";
            musicPlayer.SetMasterVolume(SliderToVolume(intVal));
        }

        private void OnSFXSliderChanged(float value)
        {
            int intVal = Mathf.RoundToInt(value);
            sfxValueText.text = $"SFX: {intVal.ToString()}";
            musicPlayer.SetSFXVolume(SliderToVolume(intVal));
        }

        private void OnMusicSliderChanged(float value)
        {
            int intVal = Mathf.RoundToInt(value);
            musicValueText.text = $"Music: {intVal.ToString()}";
            musicPlayer.SetMusicVolume(SliderToVolume(intVal));
        }

        public void IncrementSlider(Slider slider)
        {
            int oldValue = Mathf.RoundToInt(slider.value);
            int newValue = Mathf.Clamp(oldValue + 1, 0, 10);
            slider.value = newValue;
        }

        public void DecrementSlider(Slider slider)
        {
            int oldValue = Mathf.RoundToInt(slider.value);
            int newValue = Mathf.Clamp(oldValue - 1, 0, 10);
            slider.value = newValue;
        }

      
    }
}

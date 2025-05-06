using UnityEngine;
using UnityEngine.Audio;

namespace Music
{
    public class MusicPlayer : MonoBehaviour
    {
        private static MusicPlayer instance;
        private AudioSource audioSource;

        [Header("Mixer")]
        public AudioMixer audioMixer; // ← aquí arrastras tu Audio Mixer desde el Inspector

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.Play();
        }

        public void SetMasterVolume(float value)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        }

        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        }

        public void SetSFXVolume(float value)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        }
    }
}
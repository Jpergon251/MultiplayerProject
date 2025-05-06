using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Asegúrate de tener esto si usas TextMeshPro

namespace MenuScripts
{
    public class GameOverMenu : MonoBehaviour
    {
        public GameObject title;
        public TextMeshProUGUI counterText; // Arrástralo desde el inspector

        public GameObject inGameMenu;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            
                StartCoroutine(CountDown());    
            
            
        }

        private IEnumerator CountDown()
        {
            int count = 9;

            while (count >= 0)
            {
                counterText.text = count.ToString();
                yield return new WaitForSeconds(1f);
                count--;
            }

            // Cargar la escena principal al terminar la cuenta regresiva
            GoToMainMenu();
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("StartMenu");
        }
        
    }
}
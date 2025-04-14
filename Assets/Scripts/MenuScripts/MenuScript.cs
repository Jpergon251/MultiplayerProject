using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace MenuScripts
{
    public class MenuScript : MonoBehaviour
    {
        public Button singlePlayerButton;
        public Button multiPlayerButton;
        public Button optionsButton;
        public Button exitButton;
        
        public GameObject startMenu;
        public GameObject mainMenu;

        private void Start()
        {
            startMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "StartMenu");
            mainMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "MainMenu");
            if (startMenu != null) startMenu.SetActive(true);
            if (mainMenu != null) mainMenu.SetActive(false);
        }

        public void CloseGame()
        {
            Application.Quit();
        }

        public void StartGameSinglePlayer()
        {
            SceneManager.LoadScene("GameSceneSinglePlayer");
        }
     
    }
}

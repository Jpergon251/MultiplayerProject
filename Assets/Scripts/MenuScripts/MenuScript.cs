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
        public GameObject lobbyMenu;

        private void Start()
        {
            startMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "StartMenu");
            mainMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "MainMenu");
            lobbyMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "LobbyMenu");
            if (startMenu != null) startMenu.SetActive(true);
            if (mainMenu != null) mainMenu.SetActive(false);
            if (lobbyMenu != null) lobbyMenu.SetActive(false);
        }

        public void CloseGame()
        {
            Application.Quit();
        }

        public void LobbyMenuActive()
        {
            if (mainMenu.activeSelf)
            {
                mainMenu.SetActive(false);
                lobbyMenu.SetActive(true);
            }
        }

        public void CloseLobbyMenu()
        {
            if (lobbyMenu.activeSelf)
            {
                lobbyMenu.SetActive(false);
                mainMenu.SetActive(true);
            }
        }
        public void StartGameSinglePlayer()
        {
            SceneManager.LoadScene("GameSceneSinglePlayer");
        }
     
    }
}

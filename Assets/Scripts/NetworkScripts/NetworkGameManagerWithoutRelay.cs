using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkScripts
{
    public class NetworkGameManagerWithoutRelay : MonoBehaviour
    {
        public static NetworkGameManagerWithoutRelay Instance;

        private void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else { Destroy(gameObject); }
        }

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            SceneManager.LoadScene("GameScene");
        }

        public void StartClient(string ip)
        {
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            transport.SetConnectionData(ip, 7777); // puerto por defecto
            NetworkManager.Singleton.StartClient();
        }

        public void StartServer()
        {
            NetworkManager.Singleton.StartServer();
        }
    }
}

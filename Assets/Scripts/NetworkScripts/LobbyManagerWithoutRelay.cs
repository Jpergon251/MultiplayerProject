using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkScripts
{
    public class LobbyManagerWithoutRelay : MonoBehaviour
    {
        public static LobbyManagerWithoutRelay Instance;

        [Header("UI Manager")]
        public TMP_Text lobbyCode;
        public TMP_InputField inputLobbyCode;  // Campo para ingresar el ID del lobby
        public GameObject[] playerSlots; // Asegúrate de que tenga 4 elementos (uno por cada jugador)
        public GameObject startButton;
        
        private Lobby _hostLobby;
        private Lobby _joinedLobby;
        private float _heartbeatTimer;
        private float _lobbyUpdateTimer;
        private string _lobbyCode;
        private string _playerName ;
        
        
        
        private async void Start()
        {
            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed In: {AuthenticationService.Instance.PlayerId}");
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            GenerateRandomName();
            Debug.Log(_playerName);
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
            HandleLobbyPollForUpdates();
        }
        public void UpdateLobbyUI()
        {
            if (_joinedLobby == null || playerSlots == null || playerSlots.Length == 0)
            {
                Debug.LogWarning("No se puede actualizar la UI: Lobby o slots no definidos.");
                return;
            }

            // Actualiza los nombres de los jugadores
            for (int i = 0; i < playerSlots.Length; i++)
            {
                GameObject slot = playerSlots[i];
                TMP_Text nameText = slot.GetComponentInChildren<TMP_Text>();

                if (i < _joinedLobby.Players.Count)
                {
                    var player = _joinedLobby.Players[i];
                    string playerName = player.Data != null && player.Data.ContainsKey("PlayerName")
                        ? player.Data["PlayerName"].Value
                        : "SinNombre";
                    nameText.text = playerName;
                }
                else
                {
                    nameText.text = "Waiting for player...";
                }
            }

            // Mostrar u ocultar elementos según el rol
            string localPlayerId = AuthenticationService.Instance.PlayerId;
            bool isHost = _joinedLobby.HostId == localPlayerId;

            if (startButton != null)
                startButton.SetActive(isHost);

            
        }
        private async void HandleLobbyHeartbeat()
        {
            if (_hostLobby != null)
            {
                _heartbeatTimer -= Time.deltaTime;
                if (_heartbeatTimer < 0f)
                {
                    float heartbeatTimerMax = 15;
                    _heartbeatTimer = heartbeatTimerMax;

                    await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
                    
                }
                
            }
        }

        private async void HandleLobbyPollForUpdates()
        {
            if (_joinedLobby != null)
            {
                _lobbyUpdateTimer -= Time.deltaTime;
                if (_lobbyUpdateTimer < 0f)
                {
                    float _lobbyUpdateTimerMax = 1.1f;
                    _lobbyUpdateTimer = _lobbyUpdateTimerMax;
                    
                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id); 
                    _joinedLobby = lobby;
                    UpdateLobbyUI();
                }
            }
        }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        }

        private void GenerateRandomName()
        {
            string[] nouns = {
                "Zapato", "Toalla", "Oso", "Cazador", "Luchador", "Robot", "Espejo", "Sombrero",
                "Cuchara", "Fantasma", "Martillo", "Papel", "Lampara", "Buho", "Tren", "Mesa", "Nube"
            };

            string[] adjectives = {
                "Agresivo", "Risueño", "Rimbombante", "Amoroso", "Valiente", "Silencioso", "Torpe", "Radiante",
                "Explosivo", "Curioso", "Inquieto", "Elegante", "Salvaje", "Tranquilo", "Poderoso"
            };

            string randomNoun = nouns[UnityEngine.Random.Range(0, nouns.Length)];
            string randomAdjective = adjectives[UnityEngine.Random.Range(0, adjectives.Length)];
            int number = UnityEngine.Random.Range(10, 100);

            _playerName = randomNoun + randomAdjective + number;
        }

        public async void CreateLobby()
        {
            try
            {
                string localIP = GetLocalIPAddress(); // Nueva función, la agregamos abajo
                Debug.Log("Mi IP local: " + localIP);

                string lobbyName = "MiLobby";
                int maxPlayers = 4;

                Player hostPlayer = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {
                            "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName)
                        }
                    }
                };

                var options = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Player = hostPlayer,
                    Data = new Dictionary<string, DataObject>
                    {
                        { "hostIP", new DataObject(DataObject.VisibilityOptions.Member, localIP) }
                    }
                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                _hostLobby = lobby;
                _joinedLobby = lobby;
                _lobbyCode = lobby.LobbyCode;

                lobbyCode.text = _lobbyCode;
                Debug.Log($"Lobby creado con IP {localIP} y código {_lobbyCode}");
                
                if (!NetworkManager.Singleton.IsListening)
                {
                    NetworkManager.Singleton.StartHost();
                    Debug.Log("Host iniciado.");
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Error al crear el lobby: {e.Message}");
            }
        }
        private string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }
        public async void JoinLobby()
        {
            try
            {
                string code = inputLobbyCode.text.ToUpper();

                Player joiningPlayer = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName) }
                    }
                };

                JoinLobbyByCodeOptions joinOptions = new JoinLobbyByCodeOptions
                {
                    Player = joiningPlayer
                };

                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinOptions);
                _joinedLobby = lobby;
                _lobbyCode = lobby.LobbyCode;

                lobbyCode.text = _lobbyCode;

                // Aquí lees la IP del host que guardaste en los datos del lobby
                string hostIP = null;
                if (lobby.Data != null && lobby.Data.ContainsKey("hostIP"))
                {
                    hostIP = lobby.Data["hostIP"].Value;
                }

                if (!string.IsNullOrEmpty(hostIP))
                {
                    var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                    transport.SetConnectionData(hostIP, 7777);
            
                    // Ahora sí arrancas el cliente para conectarte al host
                    NetworkManager.Singleton.StartClient();
                }
                else
                {
                    Debug.LogError("No se encontró la IP del host en los datos del lobby.");
                }

                Debug.Log($"Te has unido al lobby {code} como {_playerName}");
                
                if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
                {
                    NetworkManager.Singleton.StartClient();
                    Debug.Log("Cliente iniciado y conectado al host.");
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Error al unirse al lobby: {e.Message}");
            }
        }
        public async void LeaveLobby()
        {
            try
            {
                string playerId = AuthenticationService.Instance.PlayerId;

                // Si eres el host
                if (_hostLobby != null && _hostLobby.HostId == playerId)
                {
                    await LobbyService.Instance.DeleteLobbyAsync(_hostLobby.Id);
                    Debug.Log("Lobby eliminado porque el host salió.");
                    _hostLobby = null;
                }
                // Si eres un cliente
                else if (_joinedLobby != null && _hostLobby == null)
                {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
                    Debug.Log("Saliste del lobby.");
                    _joinedLobby = null;
                }

                _lobbyCode = null;
                lobbyCode.text = ""; // Limpia la UI si quieres
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Error al salir del lobby: {e.Message}");
            }
        }
        
        public void StartGame()
        {
            if (_joinedLobby == null)
            {
                Debug.LogWarning("No estás en un lobby.");
                return;
            }

            string localPlayerId = AuthenticationService.Instance.PlayerId;
            if (_joinedLobby.HostId != localPlayerId)
            {
                Debug.LogWarning("Solo el host puede iniciar la partida.");
                return;
            }

            Debug.Log("Host inició la partida. Cambiando escena para todos...");
            NetworkManager.Singleton.SceneManager.LoadScene("GameSceneMultiplayer", LoadSceneMode.Single);
        }
        
    }
}
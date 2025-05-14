
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace NetworkScripts
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance;

        [Header("UI Manager")]
        public TMP_Text lobbyCode;
        public TMP_InputField inputLobbyCode;  // Campo para ingresar el ID del lobby
        public GameObject playersPanel;
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

            // Aquí podrías agregar más lógica según roles:
            // if (isHost) { ... }
            // else { ... }
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
                string lobbyName = "MiLobby";
                int maxPlayers = 4;

                // Crear los datos del jugador que va a ser el host
                Player hostPlayer = new Player
                {
                    Data = new System.Collections.Generic.Dictionary<string, PlayerDataObject>
                    {
                        {
                            "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName)
                        }
                    }
                };

                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = false, // o true si no quieres que aparezca en el listado
                    Player = hostPlayer
                };

                // Crear el lobby
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                _hostLobby = lobby;
                _joinedLobby = lobby; // También se une como host
                _lobbyCode = lobby.LobbyCode;

                // Mostrar el código del lobby en el UI
                lobbyCode.text = _lobbyCode;
                Debug.Log($"Lobby creado con éxito: {_lobbyCode} | Jugador: {_playerName}");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Error al crear el lobby: {e.Message}");
            }
        }
        
        public async void JoinLobby()
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_hostLobby.Id);
                _hostLobby = null;
                string code = inputLobbyCode.text.ToUpper();

                Player joiningPlayer = new Player
                {
                    Data = new System.Collections.Generic.Dictionary<string, PlayerDataObject>
                    {
                        {
                            "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName)
                        }
                    }
                };

                JoinLobbyByCodeOptions joinOptions = new JoinLobbyByCodeOptions
                {
                    Player = joiningPlayer
                };

                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinOptions);
                _joinedLobby = lobby;
                _lobbyCode = lobby.LobbyCode;

                Debug.Log($"Te has unido al lobby {code} como {_playerName}");
                lobbyCode.text = _lobbyCode;
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
                string playerId = Unity.Services.Authentication.AuthenticationService.Instance.PlayerId;

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
        public void ListPlayersInLobby()
        {
            if (_joinedLobby == null)
            {
                Debug.LogWarning("No estás en ningún lobby.");
                return;
            }

            Debug.Log($"--- Jugadores en el lobby ({_joinedLobby.Players.Count}) ---");

            foreach (var player in _joinedLobby.Players)
            {
                string playerId = player.Id;
                string playerName = player.Data.ContainsKey("PlayerName") ? player.Data["PlayerName"].Value : "SinNombre";

                Debug.Log($"ID: {playerId} | Nombre: {playerName}");
            }
        }

        public void StartGame()
        {
            
        }
    }
}
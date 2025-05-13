using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayerScripts;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace NetworkScripts
{ 
    public class LobbyManagerV2 : MonoBehaviour
    {
    
        private Lobby _hostLobby;
        private Lobby _joinedLobby;
        private float _heartbeatTimer;
        private float _lobbyUpdateTimer;
        private string _lobbyCode;
        private string _playerName ;
        
        [SerializeField] private TMP_InputField lobbyCodeInputField;
        [SerializeField] private TMP_Text lobbyCodeText;
        private async void Start()
        {
            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed In: {AuthenticationService.Instance.PlayerId}");
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            _playerName = "Player_" + UnityEngine.Random.Range(100, 999);
            Debug.Log($"Player Name: {_playerName}");
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
            HandleLobbyPollForUpdates();
        }

        private async void HandleLobbyHeartbeat()
        {
            if (_hostLobby == null)
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
                }
            }
        }
        
        public async void CreateLobby()
        {
            try
            {
                string lobbyName = "MyLobby";
                int maxPlayers = 4;
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Player = GetPlayer()
                    
                };
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
                _hostLobby = lobby;
                _lobbyCode = lobby.LobbyCode;
                lobbyCodeText.text = "ID: " + _lobbyCode;
                Debug.Log($"Created lobby: {lobbyName}\nMax Players: {maxPlayers}");
                PrintPlayers(_hostLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to create lobby: {e.Message}");
            }
        }

        public async void JoinLobby()
        {
            try
            {
                JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
                {
                    Player = GetPlayer()
                };
                Lobby joinLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCodeInputField.text, joinLobbyByCodeOptions);
                _joinedLobby = joinLobby;
                Debug.Log($"Joined lobby: {lobbyCodeInputField.text}");
                PrintPlayers(_joinedLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to join lobby: {e.Message}");
            }
            
        }

        public async void LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to leave lobby: {e.Message}");
            }
        }

        private async void KickPlayer()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, _joinedLobby.Players[1].Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to kick player: {e.Message}");
            }
        } 
        public async void ListLobbies()
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions();
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

                Debug.Log($"Retrieved lobbies: {queryResponse.Results.Count}");
                foreach (Lobby lobby in queryResponse.Results)
                {
                    Debug.Log($"Lobby name: {lobby.Name} | Max players: {lobby.MaxPlayers} | Actualplayers: {lobby.Players.Count}");

                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to retrieve lobbies: {e.Message}");
            }
        }

        private Player GetPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName)
                    }
                }
            };
        }
        private void PrintPlayers(Lobby lobby)
        {
            Debug.Log($"Players in lobby: {lobby.Players}");
            foreach (Player player in lobby.Players)
            {
                Debug.Log($"PlayerID: {player.Id} \n Playername: {player.Data["PlayerName"].Value}");
            }
        }

        private async void UpdateLobbyGameMode()
        {
            try
            {
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to update lobby game mode: {e.Message}");
            }
        }
        
        private async void UpdatePlayerName(string playerName)
        {
            
            try
            {
                _playerName = playerName;
                await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId,
                    new UpdatePlayerOptions
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName)}
                        }
                    });
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to update player: {e.Message}");
            }
        }

        private async void MigrateLobbyHost()
        {
            try
            {
                _hostLobby = await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                {
                    HostId = _joinedLobby.Players[1].Id,
                });
                _joinedLobby = _hostLobby;
                PrintPlayers(_hostLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to migrate lobby host: {e.Message}");
            }
        }

        private async void DeleteLobby()
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Failed to delete lobby: {e.Message}");
            }
        }
    }
}
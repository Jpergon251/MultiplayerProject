using Mirror;
using Steamworks;
using Steamworks.NET;
using TMPro;
using UnityEngine;

namespace NetworkScripts
{
    public class SteamLobby : MonoBehaviour
    {

        //Callback
        protected Callback<LobbyCreated_t> LobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> JoinRequest;
        protected Callback<LobbyEnter_t> LobbyEnter;
    
        //Variables
        public ulong CurrentLobbyID;
        private const string HostAddressKey = "HostAddress";
        private CustomNetworkManager _networkManager;
    
        //GameObject
        public GameObject HostButton;
        public TextMeshProUGUI LobbyNameText;
        
        public Transform playerListContainer;
        public GameObject playerNameTemplate;


        private void Start()
        {

            if (!SteamManager.Initialized)
            {
                return;
            }

            _networkManager = GetComponent<CustomNetworkManager>();
        
            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        public void HostLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
        }
    
        public void LeaveLobby()
        {
            if (_networkManager.isNetworkActive)
            {
                _networkManager.StopHost();
            }

            if (CurrentLobbyID != 0)
            {
                var lobbyID = new CSteamID(CurrentLobbyID);
                SteamMatchmaking.LeaveLobby(lobbyID);
                SteamMatchmaking.DeleteLobbyData(lobbyID, HostAddressKey);
                CurrentLobbyID = 0;
            }

            LobbyNameText.gameObject.SetActive(false);
            HostButton.SetActive(true);
        }
        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                return;
            }
        
            Debug.Log("Lobby Created Successfully");
            _networkManager.StartHost();
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name",SteamFriends.GetPersonaName().ToString() + "'S LOBBY");
        }

        private void OnJoinRequest(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("Request to join Lobby");

            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            //Everyone
            CurrentLobbyID = callback.m_ulSteamIDLobby;
            LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");
            //Client
            if(NetworkServer.active){return;}
            _networkManager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        
            _networkManager.StartClient();
            
            
        }
        
        
    }
}
